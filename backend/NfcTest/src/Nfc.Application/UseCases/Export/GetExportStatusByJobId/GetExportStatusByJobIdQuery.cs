using Nfc.Application.Export;
using Nfc.Application.UseCases.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfc.Application.UseCases.Export.GetExportStatusByJobId
{
    public class GetExportStatusByJobIdQuery : CommandRequestBase<ExportStatus>
    {
        public Guid JobIdQuery { get; set; }
    }
}
