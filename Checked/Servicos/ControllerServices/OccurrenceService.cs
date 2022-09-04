using Checked.Data;
using Checked.Models.Models.Complement;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace Checked.Servicos.ControllerServices
{
    public class OccurrenceService
    {
        private readonly CheckedDbContext _context;
        public OccurrenceService(CheckedDbContext context)
        {
            _context = context;
        }

        public async Task AddColumn(string organizationId, string title)
        {
            OccurrenceColumnComplement columnComplement = new OccurrenceColumnComplement
            {
                OrganizationId = organizationId,
                ColumnTitle = title
            };
            _context.Add(columnComplement);
            await _context.SaveChangesAsync();
        }

        public async Task AddOccurrenceValueComplement(int column, string occurrenceId, string value)
        {
            OcurrenceComplement ocurrenceComplement = new OcurrenceComplement
            {
                OccurrenceId = occurrenceId,
                OccurrenceColumnComplementId = column,
                Value = value
            };
            _context.Add(ocurrenceComplement);
            await _context.SaveChangesAsync();
        }

        public async Task<bool[]> EditValueComplement(string[] idComplements, string[] newValue, string occurrenceID = "")
        {
            bool[] bools = new bool[idComplements.Length];
            if(idComplements.Length != 0)
            {
                int[] ids = Array.ConvertAll(idComplements,s => int.Parse(s));
                for(int i = 0; i < ids.Length; i++)
                {
                    var ocurrenceComplement = await _context.OcurrenceComplements
                        .Where(c => c.Id == ids[i])
                        .Where(c => c.OccurrenceId == occurrenceID)
                        .FirstOrDefaultAsync();
                    if(ocurrenceComplement != null)
                    {
                        ocurrenceComplement.Value = newValue[i];
                        _context.Update(ocurrenceComplement);
                        _context.SaveChanges();
                        bools[i] = true;
                    }
                    else
                    {
                        OcurrenceComplement ocComplement = new OcurrenceComplement()
                        {
                            OccurrenceId = occurrenceID,
                            Value = newValue[i],
                            OccurrenceColumnComplementId = ids[i]
                        };
                        _context.Add(ocComplement);
                        await _context.SaveChangesAsync();
                    }                    
                }
            }  
            return bools;
        }

        public async Task<List<OccurrenceColumnComplement>> GetColumnsAsync(string organizationId)
        {
            List< OccurrenceColumnComplement> result = await _context.occurrenceColumnComplements
                .Where(c => c.OrganizationId == organizationId)                
                .ToListAsync();
            return result;
        }

        public async Task<List<OcurrenceComplement>> GetInfoComplementOccurrence(string occurrenceId)
        {
            List<OcurrenceComplement> result = await _context.OcurrenceComplements
                .Where(c => c.OccurrenceId == occurrenceId)
                .Include(i =>i.OccurrenceColumnComplement)
                .ToListAsync();
            return result;
        }

        public StringBuilder SqlConsult(Dictionary<string,string> queryItens)
        {
            StringBuilder sql = new StringBuilder();
            foreach (var item in queryItens)
            {
                switch (item.Value)
                {
                    case "TP_OcorrenciaId":
                        sql.Append($" AND A.\"{item.Key}\" = {item.Value}");
                        break;
                    case "StatusId":
                        sql.Append($" AND A.\"{item.Key}\" = {item.Value}");
                        break;
                    case "CurrentFilter":
                        break;
                    default:
                        sql.Append($" AND A.\"{item.Key}\" = '{item.Value}'");
                        break;
                }
            }
            return sql;
        }
        public string UrlCurrentFilter(Dictionary<string, string> queryItens)
        {
            StringBuilder urlQuery = new StringBuilder();
            foreach (var item in queryItens)
            {
                if(item.Key != "PageNumber")
                {
                    urlQuery.Append($"&{item.Key}={item.Value}");
                }                
            }
            return urlQuery.ToString();

        }
    }
}
