﻿using System;



namespace blqw
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    static class Extensions
    {
        /// <summary>
        /// 判断类型是否是静态类
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsStatic(this Type type) => type != null && type.IsAbstract && type.IsSealed;
    }
}
