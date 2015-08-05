// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Format.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CreateGuid
{
    /// <summary>
    /// The Guid Format
    /// </summary>
    public enum GuidFormat
    {
        /// <summary>
        /// 32 digits separated by hyphens, enclosed in braces: 
        /// {00000000-0000-0000-0000-000000000000} 
        /// </summary>
        B, 

        /// <summary>
        /// 32 digits separated by hyphens: 
        /// 00000000-0000-0000-0000-000000000000 
        /// </summary>
        D, 

        /// <summary>
        /// 32 digits: 
        /// 00000000000000000000000000000000 
        /// </summary>
        N, 

        /// <summary>
        /// Four hexadecimal values enclosed in braces, where the fourth value is a subset of eight hexadecimal values that is also enclosed in braces:
        /// {0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}
        /// </summary>
        X, 

        /// <summary>
        /// 32 digits separated by hyphens, enclosed in parentheses: 
        /// (00000000-0000-0000-0000-000000000000) 
        /// </summary>
        P
    };
}