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
            var result = await _context.Occurrences
                   .Include(o => o.Status)
                   .Include(o => o.Tp_Ocorrencia)
                   .Include(o => o.Tp_Ocorrencia)
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

            foreach (var item in result)
            {
                if (keyValuePairs.ContainsKey(item.Status.Name))
                {
                    List<string> obj = new List<string>();
                    keyValuePairs.TryGetValue(item.Status.Name, out obj);
                    obj.Add($"{item.Tp_Ocorrencia.Name.ToLower()},{item.Description.ToLower()}");
                    keyValuePairs[item.Status.Name] = obj;
                }
                else
                {
                    List<string> obj = new List<string>();
                    obj.Add($"{item.Tp_Ocorrencia.Name.ToLower()},{item.Description.ToLower()}");
                    keyValuePairs.Add(item.Status.Name, obj);
                }
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
                            obj.Add($"{item.Subject.ToLower()},{item.Objective.ToLower()}");
                            keyValuePairs["Atrasado"] = obj;
                        }
                        else
                        {
                            List<string> obj = new List<string>();
                            obj.Add($"{item.Subject.ToLower()},{item.Objective.ToLower()}");
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
                            obj.Add($"{item.Subject.ToLower()},{item.Objective.ToLower()}");
                            keyValuePairs["Em tempo"] = obj;
                        }
                        else
                        {
                            List<string> obj = new List<string>();
                            obj.Add($"{item.Subject.ToLower()},{item.Objective.ToLower()}");
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
                            obj.Add($"{item.Subject.ToLower()},{item.Objective.ToLower()}");
                            keyValuePairs["Próximo do Vencimento"] = obj;
                        }
                        else
                        {
                            List<string> obj = new List<string>();
                            obj.Add($"{item.Subject.ToLower()},{item.Objective.ToLower()}");
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
                        obj.Add($"{item.What.ToLower()},{item.NewFinish}");
                        keyValuePairs["Encerrado"] = obj;
                    }
                    else
                    {
                        List<string> obj = new List<string>();
                        obj.Add($"{item.What.ToLower()},{item.NewFinish}");
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
                        obj.Add($"{item.What.ToLower()},{item.NewFinish}");
                        keyValuePairs["Atrasado"] = obj;
                    }
                    else
                    {
                        List<string> obj = new List<string>();
                        obj.Add($"{item.What.ToLower()},{item.NewFinish}");
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
                        obj.Add($"{item.What.ToLower()},Vencimento em: {item.NewFinish}");
                        keyValuePairs["Em tempo"] = obj;
                    }
                    else
                    {
                        List<string> obj = new List<string>();
                        obj.Add($"{item.What.ToLower()},{item.NewFinish}");
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
                        obj.Add($"{item.What.ToLower()},{item.NewFinish}");
                        keyValuePairs["Próximo do Vencimento"] = obj;
                    }
                    else
                    {
                        List<string> obj = new List<string>();
                        obj.Add($"{item.What.ToLower()},{item.NewFinish}");
                        keyValuePairs.Add("Próximo do Vencimento", obj);
                    }
                }
            }
            return keyValuePairs;
        }

    }
}
