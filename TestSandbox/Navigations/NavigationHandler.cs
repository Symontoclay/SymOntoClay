/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TestSandbox.Navigations
{
    public class NavigationHandler
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            var wayPointsList = new List<WayPoint>();

            var a_Point = new WayPoint() { Name = "A" };
            wayPointsList.Add(a_Point);

            var b_Point = new WayPoint() { Name = "B" };
            wayPointsList.Add(b_Point);

            LinkWayPoints(a_Point, b_Point);

            var c_Point = new WayPoint() { Name = "C" };
            wayPointsList.Add(c_Point);
            LinkWayPoints(b_Point, c_Point);

            var d_Point = new WayPoint() { Name = "D" };
            wayPointsList.Add(d_Point);
            LinkWayPoints(b_Point, d_Point);
            LinkWayPoints(c_Point, d_Point);
            //-------------
            var e_Point = new WayPoint() { Name = "E" };
            wayPointsList.Add(e_Point);
            LinkWayPoints(d_Point, e_Point);
            LinkWayPoints(a_Point, e_Point);

            var f_Point = new WayPoint() { Name = "F" };
            wayPointsList.Add(f_Point);
            LinkWayPoints(e_Point, f_Point);
            LinkWayPoints(c_Point, f_Point);

            var g_Point = new WayPoint() { Name = "G" };
            wayPointsList.Add(g_Point);
            LinkWayPoints(f_Point, g_Point);
            LinkWayPoints(d_Point, f_Point);
            LinkWayPoints(a_Point, g_Point);

            var h_Point = new WayPoint() { Name = "H" };
            wayPointsList.Add(h_Point);

            LinkWayPoints(g_Point, h_Point);
            LinkWayPoints(a_Point, h_Point);
            LinkWayPoints(b_Point, h_Point);
            LinkWayPoints(f_Point, h_Point);
            LinkWayPoints(c_Point, h_Point);
            LinkWayPoints(e_Point, h_Point);

            var i_Point = new WayPoint() { Name = "I" };
            wayPointsList.Add(i_Point);

            LinkWayPoints(h_Point, i_Point);
            LinkWayPoints(b_Point, i_Point);
            LinkWayPoints(e_Point, i_Point);

            var j_Point = new WayPoint() { Name = "J" };
            wayPointsList.Add(j_Point);
            LinkWayPoints(b_Point, j_Point);
            LinkWayPoints(c_Point, j_Point);

            LinkWayPoints(i_Point, j_Point);
            LinkWayPoints(a_Point, j_Point);
            LinkWayPoints(e_Point, j_Point);
            //-------------
            var k_Point = new WayPoint() { Name = "K" };
            wayPointsList.Add(k_Point);

            LinkWayPoints(j_Point, k_Point);
            LinkWayPoints(c_Point, k_Point);
            LinkWayPoints(e_Point, k_Point);

            var l_Point = new WayPoint() { Name = "L" };
            wayPointsList.Add(l_Point);
            LinkWayPoints(k_Point, l_Point);
            LinkWayPoints(a_Point, l_Point);
            LinkWayPoints(b_Point, l_Point);
            LinkWayPoints(c_Point, l_Point);

            var m_Point = new WayPoint() { Name = "M" };
            wayPointsList.Add(m_Point);
            LinkWayPoints(l_Point, m_Point);
            LinkWayPoints(c_Point, m_Point);
            LinkWayPoints(e_Point, m_Point);

            var n_Point = new WayPoint() { Name = "N" };
            wayPointsList.Add(n_Point);
            LinkWayPoints(m_Point, n_Point);
            LinkWayPoints(a_Point, n_Point);
            LinkWayPoints(b_Point, n_Point);
            LinkWayPoints(c_Point, n_Point);

            var o_Point = new WayPoint() { Name = "O" };
            wayPointsList.Add(o_Point);
            LinkWayPoints(n_Point, o_Point);
            LinkWayPoints(a_Point, o_Point);
            LinkWayPoints(e_Point, o_Point);

            var p_Point = new WayPoint() { Name = "P" };
            wayPointsList.Add(p_Point);
            LinkWayPoints(o_Point, p_Point);
            LinkWayPoints(a_Point, p_Point);
            LinkWayPoints(b_Point, p_Point);
            LinkWayPoints(c_Point, p_Point);

            _logger.Info($"wayPointsList.Count = {wayPointsList.Count}");

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            //var pathsList = CalculateNavPathsDFS(wayPointsList);
            var pathsList = CalculateNavPathsBFS(wayPointsList);

            stopWatch.Stop();

            _logger.Info($"stopWatch.Elapsed = {stopWatch.Elapsed}");

            _logger.Info($"pathsList.Count = {pathsList.Count}");

            //foreach (var path in pathsList)
            //{
            //    _logger.Info($"path = {path}");
            //}

            _logger.Info("End");
        }

        private static List<NavPath> CalculateNavPathsBFS(List<WayPoint> wayPointsList)
        {
            var result = new List<NavPath>();

            if (!wayPointsList.Any())
            {
                return result;
            }

            var cache = new Dictionary<WayPoint, Dictionary<WayPoint, List<List<WayPoint>>>>();

            var n = 0;

            var visitedWayPointsList = new List<WayPoint>();

            while (true)
            {
                var targetWayPointsList = new List<WayPoint>() { wayPointsList.First() };

                while (targetWayPointsList.Any())
                {
                    //_logger.Info($"targetWayPointsList.Count = {targetWayPointsList.Count}");
                    //_logger.Info($"targetWayPointsList = {string.Join(",", targetWayPointsList.Select(p => p.Name))}");
                    //_logger.Info($"visitedWayPointsList = {string.Join(",", visitedWayPointsList.Select(p => p.Name))}");

                    var newTargetWayPointsList = new List<WayPoint>();

                    foreach (var targetWayPoint in targetWayPointsList)
                    {
                        n++;
                        newTargetWayPointsList.AddRange(ProcessWayPointBFS(targetWayPoint, result, visitedWayPointsList, cache));
                    }

                    targetWayPointsList = newTargetWayPointsList;
                }

                wayPointsList = wayPointsList.Where(p => !visitedWayPointsList.Contains(p)).ToList();

                if (!wayPointsList.Any())
                {
                    //_logger.Info("!wayPointsList.Any()");

                    break;
                }
            }

            _logger.Info($"n = {n}");

            return result;
        }

        private static List<WayPoint> ProcessWayPointBFS(WayPoint wayPoint, List<NavPath> result, List<WayPoint> visitedWayPointsList, Dictionary<WayPoint, Dictionary<WayPoint, List<List<WayPoint>>>> cache)
        {
            var newTargetWayPointsList = new List<WayPoint>();

            if (visitedWayPointsList.Contains(wayPoint))
            {
                return newTargetWayPointsList;
            }

            //_logger.Info($"wayPoint = {wayPoint}");

            visitedWayPointsList.Add(wayPoint);

            Dictionary<WayPoint, List<List<WayPoint>>> targetCacheDict = null;

            if (cache.ContainsKey(wayPoint))
            {
                targetCacheDict = cache[wayPoint];
            }
            else
            {
                targetCacheDict = new Dictionary<WayPoint, List<List<WayPoint>>>();
                cache[wayPoint] = targetCacheDict;
            }

            //_logger.Info($"targetCacheDict.Count = {targetCacheDict.Count}");

            foreach (var link in wayPoint.Links)
            {
                WayPoint targetWayPoint = null;

                if (link.A == wayPoint)
                {
                    targetWayPoint = link.B;
                }
                else
                {
                    targetWayPoint = link.A;
                }

                //_logger.Info($"targetWayPoint = {targetWayPoint}");

                if (visitedWayPointsList.Contains(targetWayPoint))
                {
                    //_logger.Info($"targetWayPoint = {targetWayPoint} has been visited!");

                    //var prevWayPointsList = targetCacheDict[targetWayPoint];

                    //_logger.Info($"prevWayPointsList.Count = {prevWayPointsList.Count}");

                    //foreach(var targetPrevWayPointsList in prevWayPointsList)
                    //{
                    //    _logger.Info($"chain = {string.Join("<->", targetPrevWayPointsList.Select(p => p.Name))}");
                    //}

                    if (cache.ContainsKey(targetWayPoint))
                    {
                        var reverceTargetCacheDict = cache[targetWayPoint];

                        //_logger.Info($"reverceTargetCacheDict.Count = {reverceTargetCacheDict.Count}");

                        var prevWayPointsList = reverceTargetCacheDict.SelectMany(p => p.Value).Where(p => !p.Contains(wayPoint)).ToList();

                        //_logger.Info($"prevWayPointsList.Count = {prevWayPointsList.Count}");

                        foreach (var targetPrevWayPointsList in prevWayPointsList)
                        {
                            var chain = targetPrevWayPointsList.ToList();

                            //_logger.Info($"chain = {string.Join("<->", chain.Select(p => p.Name))}");

                            chain.Add(wayPoint);

                            //_logger.Info($"chain (next) = {string.Join("<->", chain.Select(p => p.Name))}");

                            if (targetCacheDict.ContainsKey(targetWayPoint))
                            {
                                targetCacheDict[targetWayPoint].Add(chain.ToList());
                            }
                            else
                            {
                                targetCacheDict[targetWayPoint] = new List<List<WayPoint>>() { chain.ToList() };
                            }

                            var navPath = new NavPath();
                            navPath.Start = wayPoint;
                            navPath.End = targetWayPoint;
                            navPath.Corners = chain.ToList();
                            result.Add(navPath);

                            var reverseNavPath = new NavPath();
                            reverseNavPath.Start = targetWayPoint;
                            reverseNavPath.End = wayPoint;

                            chain.Reverse();

                            reverseNavPath.Corners = chain.ToList();
                            result.Add(reverseNavPath);

                            if (cache.ContainsKey(targetWayPoint))
                            {
                                var reverceTargetCacheDict_2 = cache[targetWayPoint];

                                if (reverceTargetCacheDict_2.ContainsKey(wayPoint))
                                {
                                    reverceTargetCacheDict_2[wayPoint].Add(chain);
                                }
                                else
                                {
                                    reverceTargetCacheDict_2[wayPoint] = new List<List<WayPoint>>() { chain };
                                }
                            }
                            else
                            {
                                var reverceTargetCacheDict_2 = new Dictionary<WayPoint, List<List<WayPoint>>>();
                                cache[targetWayPoint] = reverceTargetCacheDict_2;
                                reverceTargetCacheDict_2[wayPoint] = new List<List<WayPoint>>() { chain };
                            }
                        }
                    }

                    continue;
                }
                else
                {
                    if (targetCacheDict.ContainsKey(targetWayPoint))
                    {
                        continue;
                    }

                    var chain = new List<WayPoint>() { wayPoint, targetWayPoint };

                    targetCacheDict[targetWayPoint] = new List<List<WayPoint>>() { chain.ToList() };

                    var navPath = new NavPath();
                    navPath.Start = wayPoint;
                    navPath.End = targetWayPoint;
                    navPath.Corners = chain.ToList();
                    result.Add(navPath);

                    var reverseNavPath = new NavPath();
                    reverseNavPath.Start = targetWayPoint;
                    reverseNavPath.End = wayPoint;

                    chain.Reverse();

                    reverseNavPath.Corners = chain.ToList();
                    result.Add(reverseNavPath);

                    if (cache.ContainsKey(targetWayPoint))
                    {
                        var reverceTargetCacheDict = cache[targetWayPoint];

                        if (reverceTargetCacheDict.ContainsKey(wayPoint))
                        {
                            reverceTargetCacheDict[wayPoint].Add(chain);

                            throw new NotImplementedException();
                        }
                        else
                        {
                            reverceTargetCacheDict[wayPoint] = new List<List<WayPoint>>() { chain };
                        }
                    }
                    else
                    {
                        var reverceTargetCacheDict = new Dictionary<WayPoint, List<List<WayPoint>>>();
                        cache[targetWayPoint] = reverceTargetCacheDict;
                        reverceTargetCacheDict[wayPoint] = new List<List<WayPoint>>() { chain };
                    }

                    newTargetWayPointsList.Add(targetWayPoint);
                }
            }

            return newTargetWayPointsList;
        }

        private static List<NavPath> CalculateNavPathsDFS(List<WayPoint> wayPointsList)
        {
            var result = new List<NavPath>();

            if (!wayPointsList.Any())
            {
                return result;
            }

            var visitedWayPointsList = new List<WayPoint>();

            var cache = new Dictionary<WayPoint, Dictionary<WayPoint, List<WayPoint>>>();

            var n = 0;

            foreach (var wayPoint in wayPointsList)
            {
                var chain = new List<WayPoint>();
                chain.Add(wayPoint);

                ProcessWayPointDFS(wayPoint, result, visitedWayPointsList, chain, cache, ref n);

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            _logger.Info($"n = {n}");

            //var initResultList = result.ToList();

            //foreach (var initResult in initResultList)
            //{
            //    var additionalResult = new NavPath();
            //    additionalResult.Start = initResult.End;
            //    additionalResult.End = initResult.Start;
            //    additionalResult.Corners = initResult.Corners.ToList();
            //    additionalResult.Corners.Reverse();
            //    result.Add(additionalResult);
            //}

            return result;
        }

        private static void ProcessWayPointDFS(WayPoint wayPoint, List<NavPath> result, List<WayPoint> visitedWayPointsList, List<WayPoint> chain, Dictionary<WayPoint, Dictionary<WayPoint, List<WayPoint>>> cache, ref int n)
        {
            _logger.Info($"wayPoint = {wayPoint}");
            _logger.Info($"chain = {string.Join("<->", chain.Select(p => p.Name))}");

            n++;

            if (visitedWayPointsList.Contains(wayPoint))
            {
                _logger.Info($"wayPoint = {wayPoint} has been visited!");

                //return;
            }

            visitedWayPointsList.Add(wayPoint);

            var startWayPoint = chain.First();

            foreach (var link in wayPoint.Links)
            {
                WayPoint targetWayPoint = null;

                if (link.A == wayPoint)
                {
                    targetWayPoint = link.B;
                }
                else
                {
                    targetWayPoint = link.A;
                }

                if (startWayPoint == targetWayPoint)
                {
                    continue;
                }

                if (chain.Contains(targetWayPoint))
                {
                    continue;
                }

                //if(cache.ContainsKey(wayPoint))
                //{
                //    var cacheEntry = cache[wayPoint];

                //    if(cacheEntry.ContainsKey(targetWayPoint))
                //    {
                //        _logger.Info($"hit into cache = wayPoint = {wayPoint}");
                //    }
                //}

                var newChain = chain.ToList();

                newChain.Add(targetWayPoint);

                var navPath = new NavPath();
                navPath.Start = startWayPoint;
                navPath.End = targetWayPoint;
                navPath.Corners = newChain.ToList();
                result.Add(navPath);

                ProcessWayPointDFS(targetWayPoint, result, visitedWayPointsList, newChain, cache, ref n);

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public static void LinkWayPoints(WayPoint wayPoint1, WayPoint wayPoint2)
        {
            var link = new Link();
            wayPoint1.Links.Add(link);
            link.A = wayPoint1;

            wayPoint2.Links.Add(link);
            link.B = wayPoint2;
        }
    }
}

