using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.filtering
{
    public enum Operators
    {
        EQUALS = 0,
        NOT_EQUAL = 1,
        LESS_THAN = 2,
        GREATER_THAN = 3,
        LESS_THAN_EQUAL = 4,
        GREATER_THAN_EQUAL = 5,
        STARTS_WITH = 6,
        ENDS_WITH = 7,
        CONTAINS = 8,
        DOES_NOT_STARTS_WITH = 9,
        DOES_NOT_ENDS_WITH = 10,
        DOES_NOT_CONTAINS = 11,

        AND = 12,
        OR = 13
    }
}
