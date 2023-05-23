using System;

namespace Server.Log
{
    /// <summary>
    /// Operate Level
    /// </summary>
    public enum OperateLevel
    {
        /// <summary>
        /// なんでも
        /// </summary>
        EVERYTHING = 0,
        /// <summary>
        /// 微々たる
        /// </summary>
        INSIGNIFICANT = 1,

        /// <summary>
        /// 影響が小さい
        /// </summary>
        SMALLEFFECT = 2,

        /// <summary>
        /// 普通
        /// </summary>
        NORMAL = 3,

        /// <summary>
        /// 重要
        /// </summary>
        IMPORTTANT = 4
    }
}
