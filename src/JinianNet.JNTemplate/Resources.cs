/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Collections.ObjectModel;

namespace JinianNet.JNTemplate
{
    /// <summary>
    ///资源操作
    /// </summary>
    public class Resources
    {
        private static Collection<String> paths = new Collection<String>();

        /// <summary>
        /// 资源默认搜索路径
        /// </summary>
        public static Collection<String> Paths
        {
            get
            {
                return paths;
            }
        }

        /// <summary>
        /// 合并集合
        /// </summary>
        /// <param name="oldPaths">原路径集合</param>
        /// <param name="newPaths">待合并的路径集合</param>
        /// <returns>新的路径集合</returns>
        public static IEnumerable<String> MergerPaths(IEnumerable<String> oldPaths, params String[] newPaths)
        {
            List<String> list = new List<String>();
            list.AddRange(newPaths);
            list.AddRange(oldPaths);
            return list;
        }

        /// <summary>
        /// 查找指定文件
        /// </summary>
        /// <param name="filename">文件名 允许相对路径.路径分隔符只能使用/</param>
        /// <param name="fullPath">查找结果：完整路径</param>
        /// <returns></returns>
        public static Int32 FindPath(String filename, out String fullPath)
        {
            return FindPath(Paths, filename, out fullPath);
        }

        /// <summary>
        /// 查找指定文件
        /// </summary>
        /// <param name="paths">检索路径</param>
        /// <param name="filename">文件名 允许相对路径.路径分隔符只能使用/</param>
        /// <param name="fullPath">查找结果：完整路径</param>
        /// <returns></returns>
        public static Int32 FindPath(IEnumerable<String> paths, String filename, out String fullPath)
        {
            //filename 允许单纯的文件名或相对路径
            fullPath = null;

            if (!String.IsNullOrEmpty(filename))
            {
                filename = NormalizePath(filename);

                if (filename == null) //路径非法，比如用户试图跳出当前目录时（../header.txt）
                {
                    return -1;
                }

                //String sc = String.Empty.PadLeft(2, System.IO.Path.DirectorySeparatorChar);
                Int32 i = 0;
                foreach (String checkUrl in paths)
                {
                    if (checkUrl[checkUrl.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                        fullPath = String.Concat(checkUrl, filename);
                    else
                        fullPath = String.Concat(checkUrl.Remove(checkUrl.Length - 1, 1), filename);
                    if (System.IO.File.Exists(fullPath))
                        return i;
                }

            }
            return -1;
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="paths">检索路径</param>
        /// <param name="filename">文件名</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static String LoadResource(IEnumerable<String> paths, String filename, Encoding encoding)
        {
            String full;
            if (FindPath(paths, filename, out full) != -1)
            {
                return Load(full, encoding);
            }
            return null;
        }


        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static String LoadResource(String filename, Encoding encoding)
        {
            return LoadResource(Paths, filename, encoding);
        }

        /// <summary>
        /// 载入文件
        /// </summary>
        /// <param name="filename">完整文件路径</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static String Load(String filename, Encoding encoding)
        {
            return System.IO.File.ReadAllText(filename, encoding);
        }

        /// <summary>
        /// 路径处理
        /// </summary>
        /// <param name="filename">如果有目录</param>
        /// <returns></returns>
        public static String NormalizePath(String filename)
        {
            if (String.IsNullOrEmpty(filename) || filename.IndexOfAny(System.IO.Path.GetInvalidPathChars()) != -1)
                return null;

            List<String> values = new List<String>(filename.Split('/'));

            for (Int32 i = 0; i < values.Count; i++)
            {
                if (values[i] == "." || String.IsNullOrEmpty(values[i]))
                {
                    values.RemoveAt(i);
                    i--;
                }
                else if (values[i] == "..")
                {
                    if (i == 0)
                    {
                        return null;
                    }
                    values.RemoveAt(i);
                    i--;
                    values.RemoveAt(i);
                    i--;
                }
            }

            values.Insert(0, String.Empty);

            return String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), values.ToArray());
        }
    }
}