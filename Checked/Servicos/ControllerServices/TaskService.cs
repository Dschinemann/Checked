using Checked.Data;
using Checked.Models.Models;
using Checked.Models.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Checked.Servicos.ControllerServices
{
    public class TaskService
    {
        private readonly CheckedDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public TaskService(CheckedDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<Dictionary<string, List<string>>> GetOccurrencesAsync(ApplicationUser user)
        {
            var role = await _userManager.GetRolesAsync(user);
            bool ehAdiministrador = role.Any(c => c.Equals("Administrador"));
            var tpStatusOccurrences = await _context.TP_StatusOccurences.ToListAsync();
            var result = await _context.Occurrences
                   .Include(o => o.Status)
                   .Include(o => o.Tp_Ocorrencia)
                   .Select(s => new Occurrence()
                   {
                       Id = s.Id,
                       Tp_Ocorrencia = new TP_Ocorrencia() { Name = s.Tp_Ocorrencia.Name},
                       Description = s.Description,
                       OrganizationId = s.OrganizationId,
                       Status = new TP_StatusOccurence() { Name = s.Status.Name},
                       StatusId = s.StatusId
                   })
                   .ToListAsync();


            if (ehAdiministrador)
            {
                result = result.Where(c => c.OrganizationId.Equals(user.OrganizationId)).ToList();
            }
            else
            {
                result = result.Where(c => c.AppraiserId.Equals(user.Id)).ToList();
            }


            Dictionary<string, List<string>> keyValuePairs = new Dictionary<string, List<string>>();

            foreach(var status in tpStatusOccurrences)
            {
                List<string> obj = new List<string>();
                var occurrencesPerStatus = result.Where(c => c.StatusId == status.Id).ToList();
                foreach(var occurrence in occurrencesPerStatus)
                {
                    obj.Add($"{occurrence.Tp_Ocorrencia.Name.ToLower()},{occurrence.Description.ToLower()},{occurrence.Id},Occurrences,idOccurrence");
                }
                keyValuePairs.Add(status.Name, new List<string>(obj));
                obj.Clear();
            }

            return keyValuePairs;
        }

        public async Task<Dictionary<string, List<string>>> GetPlansAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            bool ehAdiministrador = roles.Any(c => c.Equals("Administrador"));

            Dictionary<string, List<string>> keyValuePairs = new Dictionary<string, List<string>>();

            var result = await _context.Plans
                .Include(o => o.Actions)
                .ToListAsync();

            if (ehAdiministrador)
            {
                result = result.Where(c => c.organizationId.Equals(user.OrganizationId)).ToList();
            }
            else
            {
                result = result.Where(c => c.AccountableId.Equals(user.Id)).ToList();
            }


            foreach (var item in result)
            {
                if (!item.Actions.Any(c => c.TP_StatusId.Equals((int)TP_StatusEnum.Encerrado) && c.TP_StatusId.Equals((int)TP_StatusEnum.Cancelado)))
                {
                    var totalDeDiasDoPlano = (int)item.Goal.Subtract(item.CreatedAt).TotalDays == 0 ? 1 : (int)item.Goal.Subtract(item.CreatedAt).TotalDays;
                    var prazo = (int)item.Goal.Subtract(DateTime.Today).TotalDays;
                    var decorrido = (int)totalDeDiasDoPlano - prazo;
                    var shelf = (decorrido / totalDeDiasDoPlano) * 100;
                    // vencido
                    if (shelf >= 65)
                    {
                        if (keyValuePairs.ContainsKey("Atrasado"))
                        {
                            List<string> obj = new List<string>();
                            keyValuePairs.TryGetValue("Atrasado", out obj);
                            obj.Add($"{item.Subject.ToLower()},{item.Objective.ToLower()},{item.Id},Plans,planId");
                            keyValuePairs["Atrasado"] = obj;
                        }
                        else
                        {
                            List<string> obj = new List<string>();
                            obj.Add($"{item.Subject.ToLower()},{item.Objective.ToLower()},{item.Id},Plans,planId");
                            keyValuePairs.Add("Atrasado", obj);
                        }
                    }
                    //em tempo
                    if (shelf >= 0 && shelf < 25)
                    {
                        if (keyValuePairs.ContainsKey("Em tempo"))
                        {
                            List<string> obj = new List<string>();
                            keyValuePairs.TryGetValue("Em tempo", out obj);
                            obj.Add($"{item.Subject.ToLower()},{item.Objective.ToLower()},{item.Id},Plans,planId");
                            keyValuePairs["Em tempo"] = obj;
                        }
                        else
                        {
                            List<string> obj = new List<string>();
                            obj.Add($"{item.Subject.ToLower()},{item.Objective.ToLower()},{item.Id},Plans,planId");
                            keyValuePairs.Add("Em tempo", obj);
                        }
                    }
                    //próximo do vencimento
                    if (shelf >= 25 && shelf < 65)
                    {
                        if (keyValuePairs.ContainsKey("Próximo do Vencimento"))
                        {
                            List<string> obj = new List<string>();
                            keyValuePairs.TryGetValue("Próximo do Vencimento", out obj);
                            obj.Add($"{item.Subject.ToLower()},{item.Objective.ToLower()},{item.Id},Plans,planId");
                            keyValuePairs["Próximo do Vencimento"] = obj;
                        }
                        else
                        {
                            List<string> obj = new List<string>();
                            obj.Add($"{item.Subject.ToLower()},{item.Objective.ToLower()},{item.Id},Plans,planId");
                            keyValuePairs.Add("Próximo do Vencimento", obj);
                        }
                    }
                }

            }

            return keyValuePairs;
        }

        public async Task<Dictionary<string, List<string>>> GetActionsAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            bool ehAdiministrador = roles.Any(c => c.Equals("Administrador"));

            Dictionary<string, List<string>> keyValuePairs = new Dictionary<string, List<string>>();

            var result = await _context.Actions
                .Include(o => o.TP_Status)                
                .ToListAsync();

            if (ehAdiministrador)
            {
                result = result.Where(c => c.OrganizationId.Equals(user.OrganizationId)).ToList();
            }
            else
            {
                result = result.Where(c => c.Who.Equals(user.Id)).ToList();
            }

            foreach (var item in result)
            {
                if (item.TP_StatusId.Equals(TP_StatusEnum.Encerrado))
                {
                    if (keyValuePairs.ContainsKey("Encerrado"))
                    {
                        List<string> obj = new List<string>();
                        keyValuePairs.TryGetValue("Encerrado", out obj);
                        obj.Add($"{item.What.ToLower()},{item.NewFinish},{item.Id},Actions,actionId");
                        keyValuePairs["Encerrado"] = obj;
                    }
                    else
                    {
                        List<string> obj = new List<string>();
                        obj.Add($"{item.What.ToLower()},{item.NewFinish},{item.Id},Actions,actionId");
                        keyValuePairs.Add("Encerrado", obj);
                    }
                }
                var totalDeDiasDoPlano = (int)item.NewFinish.Subtract(item.Init).TotalDays == 0 ? 1 : (int)item.NewFinish.Subtract(item.Init).TotalDays;
                var prazo = (int)item.NewFinish.Subtract(DateTime.Today).TotalDays;
                var decorrido = (int)totalDeDiasDoPlano - prazo;
                var shelf = (decorrido / totalDeDiasDoPlano) * 100;

                // vencido
                if (shelf >= 65)
                {
                    if (keyValuePairs.ContainsKey("Atrasado"))
                    {
                        List<string> obj = new List<string>();
                        keyValuePairs.TryGetValue("Atrasado", out obj);
                        obj.Add($"{item.What.ToLower()},{item.NewFinish},{item.Id},Actions,actionId");
                        keyValuePairs["Atrasado"] = obj;
                    }
                    else
                    {
                        List<string> obj = new List<string>();
                        obj.Add($"{item.What.ToLower()},{item.NewFinish},{item.Id},Actions,actionId");
                        keyValuePairs.Add("Atrasado", obj);
                    }
                }
                //em tempo
                if (shelf >= 0 && shelf < 25)
                {
                    if (keyValuePairs.ContainsKey("Em tempo"))
                    {
                        List<string> obj = new List<string>();
                        keyValuePairs.TryGetValue("Em tempo", out obj);
                        obj.Add($"{item.What.ToLower()},Vencimento em: {item.NewFinish},{item.Id},Actions,actionId");
                        keyValuePairs["Em tempo"] = obj;
                    }
                    else
                    {
                        List<string> obj = new List<string>();
                        obj.Add($"{item.What.ToLower()},{item.NewFinish},{item.Id},Actions,actionId");
                        keyValuePairs.Add("Em tempo", obj);
                    }
                }
                //próximo do vencimento
                if (shelf >= 25 && shelf < 65)
                {
                    if (keyValuePairs.ContainsKey("Próximo do Vencimento"))
                    {
                        List<string> obj = new List<string>();
                        keyValuePairs.TryGetValue("Próximo do Vencimento", out obj);
                        obj.Add($"{item.What.ToLower()},{item.NewFinish},{item.Id},Actions,actionId");
                        keyValuePairs["Próximo do Vencimento"] = obj;
                    }
                    else
                    {
                        List<string> obj = new List<string>();
                        obj.Add($"{item.What.ToLower()},{item.NewFinish},{item.Id},Actions,actionId");
                        keyValuePairs.Add("Próximo do Vencimento", obj);
                    }
                }
            }
            return keyValuePairs;
        }

        public async Task<Dictionary<string,List<string>>> GetPersonTasks(ApplicationUser user)
        {
            Dictionary<string, List<string>> keyValuePairs = new Dictionary<string, List<string>>();
           
            var statuStasks = await _context.TP_TaskStatus.ToListAsync();
            foreach (var status in statuStasks)
            {
                List<string> obj = new List<string>();
                var tasks = await _context.Tasks
                    .Where(c => c.StatusID == status.Id)
                    .Where(c => c.UserId == user.Id)
                    .ToListAsync();
                foreach(var task in tasks) 
                {
                    obj.Add($"{task.Title},{task.Description},{task.Id},{task.UserId},{task.StatusID}");
                }
                keyValuePairs.Add(status.Name, new List<string>(obj));
                obj.Clear();
            }
            return keyValuePairs;
        }
    }
}
