using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.Abp.Trees
{
    public class TreeCodeGenerator<TEntity> : ISingletonDependency, ITreeCodeGenerator<TEntity>
        where TEntity : class, ITree<TEntity>
    {
        private readonly TreeOptions _treeOptions;

        public virtual string Separator { get; set; } = ".";

        public TreeCodeGenerator(IOptions<TreeOptions> treeOption)
        {
            _treeOptions = treeOption.Value;
        }
        /// <summary>
        /// Creates code for given numbers.
        /// Example: if numbers are 4,2 then returns "00004.00002";
        /// </summary>
        /// <param name="numbers">Numbers</param>        
        public string Create(params int[] numbers)
        {
            if (numbers.IsNullOrEmpty())
            {
                return null;
            }
            var codeLength = (this._treeOptions.GetOrNull<TEntity>() ?? TreeOptions<TEntity>.Empty).CodeLength;
            return numbers.Select(number => number.ToString(new string('0', codeLength))).JoinAsString(Separator);
        }

        /// <summary>
        /// Appends a child code to a parent code. 
        /// Example: if parentCode = "00001", childCode = "00042" then returns "00001.00042".
        /// </summary>
        /// <param name="parentCode">Parent code. Can be null or empty if parent is a root.</param>
        /// <param name="childCode">Child code.</param>
        public string Append(string parentCode, string childCode)
        {
            if (childCode.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(childCode), "childCode can not be null or empty.");
            }

            if (parentCode.IsNullOrEmpty())
            {
                return childCode;
            }

            return parentCode + Separator + childCode;
        }

        /// <summary>
        /// Gets relative code to the parent.
        /// Example: if code = "00019.00055.00001" and parentCode = "00019" then returns "00055.00001".
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="parentCode">The parent code.</param>
        public string GetRelative(string code, string parentCode)
        {
            if (code.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            }

            if (parentCode.IsNullOrEmpty())
            {
                return code;
            }

            if (code.Length == parentCode.Length)
            {
                return null;
            }

            return code.Substring(parentCode.Length + Separator.Length);
        }

        /// <summary>
        /// Calculates next code for given code.
        /// Example: if code = "00019.00055.00001" returns "00019.00055.00002".
        /// </summary>
        /// <param name="code">The code.</param>
        public string Next(string code)
        {
            if (code.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            }

            var parentCode = GetParent(code);
            var lastUnitCode = GetLast(code);

            return Append(parentCode, Create(Convert.ToInt32(lastUnitCode) + 1));
        }

        /// <summary>
        /// Gets the last code.
        /// Example: if code = "00019.00055.00001" returns "00001".
        /// </summary>
        /// <param name="code">The code.</param>
        private string GetLast(string code)
        {
            if (code.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            }

            var splittedCode = code.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            return splittedCode.Last();
        }

        /// <summary>
        /// Gets parent code.
        /// Example: if code = "00019.00055.00001" returns "00019.00055".
        /// </summary>
        /// <param name="code">The code.</param>
        private string GetParent(string code)
        {
            if (code.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            }

            var splittedCode = code.Split(Separator);
            if (splittedCode.Length == 1)
            {
                return null;
            }

            return splittedCode.Take(splittedCode.Length - 1).JoinAsString(Separator);
        }
    }
}
