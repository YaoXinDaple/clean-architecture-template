using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Plans;
public record DateRange(DateTime Start,DateTime End,int Year)
{
}
