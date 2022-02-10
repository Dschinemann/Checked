﻿using Checked.Data;
using Checked.Models.Enums;
using Checked.Models.Models;
using Checked.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Checked.Servicos
{
    public class DashService
    {
        private readonly CheckedDbContext _context;

        public object ActionResume { get; private set; }

        public DashService(CheckedDbContext context)
        {
            _context = context;
        }
        public async Task<OccurrenceResume> GetOccurrenceAsync(int id)
        {
            var resume = await _context.Occurrences
                .Where(c => c.OrganizationId == id)
                .GroupBy(d => d.Id)
                .Select(e => new
                {
                    Count = e.Key,
                    TotalCost = e.Sum(s => s.Cost)
                })
                .ToListAsync();


            OccurrenceResume ocResume = new OccurrenceResume()
            {
                Count = resume.Count,
                TotalCost = resume.Sum(c => c.TotalCost)
            };
            return ocResume;
        }
        public async Task<Organization> GetOrganizationAsync(int id)
        {
            Organization org = await _context.Organizations
                .Where(c => c.Id == id)
                .FirstAsync();
            return org;
        }
        public async Task<List<ActionsResume>> GetCountActionsAsync(int id)
        {
            var ids = await _context.Plans
              .Where(c => c.organizationId == id)
              .Select(c => c.Id)
              .ToArrayAsync();

            var actions = await _context.Actions
                .Where(c => ids.Contains(c.PlanId))
                .GroupBy(c => c.TP_Status)
                .Select(c => new { status = c.Key, total = c.Count() })
                .ToListAsync();

            List<ActionsResume> list = new List<ActionsResume>();

            foreach (var action in actions)
            {
                list.Add(new ActionsResume
                {
                    Quantidade = action.total,
                    Tipo = action.status
                });
            }
            return list;
        }

        public async Task<PlansResume> GetCountPlansAsync(int id)
        {
            var plansEncerradas = await _context.Plans
                .Where(c => c.organizationId == id && !_context.Actions.Any(b => b.TP_Status != TP_Status.Encerrado && b.PlanId == c.Id))
                .CountAsync(c => c.organizationId == id);
            //.GroupBy(c => c.Id)
            //.Select(c => new { Quant = c.Count() })                
            //.ToListAsync();

            var countPlans = await _context.Plans
                .Where(c => c.organizationId == id)
                .CountAsync();

            var idPlans = await _context.Plans
              .Where(c => c.organizationId == id)
              .Select(c => c.Id)
              .ToArrayAsync();

            var custoPlans = await _context.Actions
                .Where(c => idPlans.Contains(c.PlanId))
                .SumAsync(c => c.HowMuch);

            PlansResume plansResume = new PlansResume()
            {
                CustoTotal = custoPlans,
                PlanCriados = countPlans,
                QuantEncerrados = (int)plansEncerradas
            };
            return plansResume;
        }

        public async Task<DeadLineActions> GetDeadlineAsync(int id)
        {
            var idPlans = await _context.Plans
                .Where(c => c.organizationId == id)
                .Select(c => c.Id)
                .ToArrayAsync();
            var result = await _context.Actions
                .Where(c => idPlans.Contains(c.PlanId))
                .ToListAsync();

            DeadLineActions deadLineActions = new DeadLineActions();

            foreach(var item in result)
            {
                if(deadLineActions.DeadLine.CompareTo(new DateTime()) == 0)
                {
                    deadLineActions.DeadLine = item.NewFinish;
                    deadLineActions.Name = item.What;
                }
                else
                {
                    if(deadLineActions.DeadLine.CompareTo(item.NewFinish) < 0 && item.TP_Status != TP_Status.Encerrado)
                    {
                        deadLineActions.DeadLine = item.NewFinish;
                        deadLineActions.Name = item.What;
                    }
                }
            }
            return deadLineActions;
        }
    }
}
