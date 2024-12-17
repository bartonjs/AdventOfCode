using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace AdventOfCode.Util
{
    public class Pathing
    {
        public delegate TCost AStarEstimator<in TWorld, in TPosition, out TCost>(
            TPosition candidate,
            TPosition end,
            TWorld world);

        public delegate IEnumerable<(TPosition Neighbor, TCost Cost)> AStarNeighbors<in TWorld, TPosition, TCost>(
            TPosition from,
            TWorld world);

        public static TCost AStar<TWorld, TPosition, TCost>(
            TWorld world,
            TPosition start,
            TPosition end,
            AStarNeighbors<TWorld, TPosition, TCost> neighbors,
            AStarEstimator<TWorld, TPosition, TCost> estimator,
            List<TPosition> pathToFill = null,
            Dictionary<TPosition, TCost> gScore = null,
            Func<TPosition, TPosition, bool> customEquals = null,
            bool allPaths = false)
            where TCost : INumber<TCost>, IMinMaxValue<TCost>
            where TPosition : IEquatable<TPosition>
        {
            PriorityQueue<TPosition, TCost> openSet = new PriorityQueue<TPosition, TCost>();
            Dictionary<TPosition, TPosition> cameFrom = (pathToFill is null) ? null : new Dictionary<TPosition, TPosition>();
            gScore ??= new Dictionary<TPosition, TCost>();

            gScore[start] = TCost.Zero;
            Dictionary<TPosition, TCost> fScore = new Dictionary<TPosition, TCost>();

            openSet.Enqueue(start, TCost.Zero);
            fScore[start] = TCost.Zero;

            TCost bestCost = TCost.MaxValue;

            while (openSet.TryDequeue(out TPosition current, out TCost dequeuedEstimate))
            {
                if (allPaths)
                {
                    if (dequeuedEstimate > bestCost)
                    {
                        break;
                    }
                }
                else
                {
                    if (current.Equals(end))
                    {
                        break;
                    }

                    if (customEquals is not null)
                    {
                        if (customEquals(current, end))
                        {
                            end = current;
                            break;
                        }
                    }
                }

                fScore.Remove(current);
                TCost currentScore = gScore[current];

                foreach ((TPosition neighbor, TCost cost) in neighbors(current, world))
                {
                    TCost tentative = currentScore + cost;

                    ref TCost neighborBest =
                        ref CollectionsMarshal.GetValueRefOrAddDefault(gScore, neighbor, out bool exists);

                    if (!exists || tentative < neighborBest)
                    {
                        neighborBest = tentative;

                        if (allPaths)
                        {
                            // If there's a custom equality applied we have to
                            // see if this end-state is the best end-state, so
                            // have to compare tentative and bestCost again.
                            //
                            // If there's no custom equality then there's only
                            // one end state, so it'd be redundant with the
                            // tentative < neighborBest check above.
                            if (customEquals is not null)
                            {
                                if (customEquals(neighbor, end))
                                {
                                    if (tentative < bestCost)
                                    {
                                        end = neighbor;
                                        bestCost = tentative;
                                    }
                                }
                            }
                            else if (neighbor.Equals(end))
                            {
                                bestCost = tentative;
                            }
                        }

                        if (cameFrom is not null)
                        {
                            cameFrom[neighbor] = current;
                        }

                        TCost estimatedCost = tentative + estimator(neighbor, end, world);

                        ref TCost currentEstimate =
                            ref CollectionsMarshal.GetValueRefOrAddDefault(fScore, neighbor, out exists);

                        if (!exists)
                        {
                            currentEstimate = estimatedCost;
                            openSet.Enqueue(neighbor, estimatedCost);
                        }
                        else if (currentEstimate > estimatedCost)
                        {
                            currentEstimate = estimatedCost;
                            ChangePriority(openSet, neighbor, estimatedCost);
                        }
                    }
                }
            }

            if (gScore.TryGetValue(end, out TCost finalCost))
            {
                if (pathToFill is not null)
                {
                    Stack<TPosition> stack = new Stack<TPosition>();
                    TPosition current = end;

                    while (true)
                    {
                        stack.Push(current);

                        if (current.Equals(start))
                        {
                            break;
                        }

                        current = cameFrom[current];
                    }

                    pathToFill.AddRange(stack);
                }

                return finalCost;
            }

            return TCost.MaxValue;
        }

        public static TScore BreadthFirstBest<TNode, TOther, TScore>(
            TNode start,
            TOther extraInfo,
            Func<TNode, TOther, IEnumerable<TNode>> children,
            Func<TNode, TOther, TScore> scorer,
            bool childrenDoNotRepeat = false)
            where TNode : IEquatable<TNode>
            where TScore : IComparable<TScore>, IMinMaxValue<TScore>
        {
            HashSet<TNode> dedup = null;
            Queue<TNode> queue = new Queue<TNode>();
            queue.Enqueue(start);
#if DEBUG
            int peakQueue = 1;
            int prunedNodes = 0;
            int processedNodes = 0;
            int promotions = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();
#endif

            if (!childrenDoNotRepeat)
            {
                dedup = new HashSet<TNode>();
            }

            TScore max = TScore.MinValue;

            while (queue.Count > 0)
            {
#if DEBUG
                processedNodes++;
#endif

                TNode item = queue.Dequeue();
                TScore curScore = scorer(item, extraInfo);

                if (curScore.CompareTo(max) > 0)
                {
                    max = curScore;
#if DEBUG
                    promotions++;
#endif
                }

                foreach (TNode next in children(item, extraInfo))
                {
                    if (childrenDoNotRepeat || dedup.Add(next))
                    {
                        queue.Enqueue(next);
#if DEBUG
                        peakQueue = Math.Max(peakQueue, queue.Count);
                    }
                    else
                    {
                        prunedNodes++;
#endif
                    }
                }
            }

#if DEBUG
            Console.WriteLine(
                $">> {nameof(BreadthFirstBest)} completed in {stopwatch.Elapsed.TotalMilliseconds:N3}ms with{Environment.NewLine}" +
                $">>   processed: {processedNodes:N0} peak {peakQueue:N0} pruned {prunedNodes:N0} promoted {promotions:N0}");
#endif

            return max;
        }

        public static (bool Found, TNode Value) BreadthFirstSearch<TWorld, TNode>(
            TWorld world,
            TNode start,
            Predicate<TNode> predicate,
            Func<TNode, TWorld, IEnumerable<TNode>> children,
            bool childrenDoNotRepeat = false,
            Dictionary<TNode, TNode> parentTracker = null)
            where TNode : IEquatable<TNode>
        {
            HashSet<TNode> dedup = null;
            Queue<TNode> queue = new Queue<TNode>(1048576 * 8);
            queue.Enqueue(start);

            if (!childrenDoNotRepeat)
            {
                dedup = new HashSet<TNode>();
            }

            while (queue.Count > 0)
            {
                TNode item = queue.Dequeue();

                if (predicate(item))
                {
                    return (true, item);
                }

                foreach (TNode next in children(item, world))
                {
                    if (childrenDoNotRepeat || dedup.Add(next))
                    {
                        if (parentTracker is not null)
                        {
                            parentTracker[next] = item;
                        }

                        queue.Enqueue(next);
                    }
                }
            }

            return (false, default(TNode));
        }

        public static TScore DepthFirstBest<TNode, TOther, TScore>(
            TNode start,
            TOther extraInfo,
            Func<TNode, TOther, IEnumerable<TNode>> children,
            Func<TNode, TOther, TScore> scorer)
            where TNode : IEquatable<TNode>
            where TScore : IComparable<TScore>, IMinMaxValue<TScore>
        {
            return DepthFirstBestCore(new(), start, extraInfo, children, scorer);
        }

        private static TScore DepthFirstBestCore<TNode, TOther, TScore>(
            Dictionary<TNode, TScore> cache,
            TNode currentNode,
            TOther extraInfo,
            Func<TNode, TOther, IEnumerable<TNode>> children,
            Func<TNode, TOther, TScore> scorer)
            where TNode : IEquatable<TNode>
            where TScore : IComparable<TScore>, IMinMaxValue<TScore>
        {
            TScore currentScore;

            if (cache.TryGetValue(currentNode, out currentScore))
            {
                return currentScore;
            }

            currentScore = scorer(currentNode, extraInfo);

            foreach (TNode child in children(currentNode, extraInfo))
            {
                TScore childScore = DepthFirstBestCore(
                    cache,
                    child,
                    extraInfo,
                    children,
                    scorer);

                if (childScore.CompareTo(currentScore) > 0)
                {
                    currentScore = childScore;
                }
            }

            cache[currentNode] = currentScore;
            return currentScore;
        }

        private static void ChangePriority<TElement, TPriority>(
            PriorityQueue<TElement, TPriority> queue,
            TElement element,
            TPriority newPriority)
            where TElement : IEquatable<TElement>
        {
            List<(TElement, TPriority)> temp = new List<(TElement, TPriority)>(queue.Count);

            foreach ((TElement Element, TPriority Priority) item in queue.UnorderedItems)
            {
                if (item.Element.Equals(element))
                {
                    temp.Add((element, newPriority));
                }
                else
                {
                    temp.Add(item);
                }
            }

            queue.Clear();
            queue.EnqueueRange(temp);
        }
    }
}
