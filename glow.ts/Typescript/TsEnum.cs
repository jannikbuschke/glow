using System.Collections.Generic;

namespace Glow.Core.Typescript
{
    public class TsEnum: BaseTsType
    {
        public override string Id
        {
            get
            {
                return FullName;
            }
            set{}
        }

        public IEnumerable<string> Values { get; set; }
        public string DefaultValue { get; set; }
        public bool IsNullable { get; set; }
    }
}