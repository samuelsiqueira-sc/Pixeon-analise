using CRM.Pixeon.Extends.Earlybound;
using CrmEarlyBound;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixeon.Extends.Models
{
    /// <summary>
    /// Modelo que representa as informações de um usuário
    /// </summary>
    public class SystemUserInfos
    {
        private Guid id;
        private string name;
        private EntityReference businsessUnit;
        private EntityReference position;
        private EntityReference manager;
        private List<EntityReference> systemUserRoles = new List<EntityReference>();
        private List<Team> teams = new List<Team>();
        private List<EntityReference> teamRoles = new List<EntityReference>();

        /// <summary>
        /// Construtor Padrão
        /// </summary>
        /// <param name="systemUser">Usuário</param>
        /// <param name="systemUserRoles">Direitos de Acesso do Usuário</param>
        /// <param name="teams">Equipes</param>
        /// <param name="teamRoles">Direitos de Acesso das Equipes do Usuário</param>
        public SystemUserInfos(SystemUser systemUser, List<EntityReference> systemUserRoles, List<Team> teams, List<EntityReference> teamRoles)
        {
            this.id = systemUser.Id;
            this.name = systemUser.FullName;
            this.businsessUnit = systemUser.BusinessUnitId;
            this.position = systemUser.PositionId;
            this.manager = systemUser.ParentSystemUserId;
            this.systemUserRoles = systemUserRoles;
            this.teams = teams;
            this.teamRoles = teamRoles;
        }

        /// <summary>
        /// Id do Usuário
        /// </summary>
        public Guid Id { get => id; }
        /// <summary>
        /// Nome do Usuário
        /// </summary>
        public string Name { get => name; }
        /// <summary>
        /// Unidade de Negócios
        /// </summary>
        public EntityReference BusinsessUnit { get => businsessUnit; }
        /// <summary>
        /// Posição do Usuário na Hierarquia de Cargos
        /// </summary>
        public EntityReference Position { get => position; }
        /// <summary>
        /// Gerente do Usuário
        /// </summary>
        public EntityReference Manager { get => manager; }
        /// <summary>
        /// Direitos de Acesso do Usuário
        /// </summary>
        public List<EntityReference> SystemUserRoles { get => systemUserRoles; }
        /// <summary>
        /// Equipes do Usuário (Id, Nome e Unidade de Negócios)
        /// </summary>
        public List<Team> Teams { get => teams; }
        /// <summary>
        /// Direitos de Acesso das Equipes em que o Usuário participa
        /// </summary>
        public List<EntityReference> TeamRoles { get => teamRoles; }

        /// <summary>
        /// To - Do
        /// </summary>
        /// <returns>x</returns>
        public override string ToString()
        {
            StringBuilder toString = new StringBuilder();

            toString.AppendLine($"Id: {Id}");
            toString.AppendLine($"Nome: {Name}");

            if (BusinsessUnit != null)
                toString.AppendLine($"Unidade de Negócios: {BusinsessUnit.Id} | {businsessUnit.Name}");

            if (Position != null)
                toString.AppendLine($"Cargo: {Position.Id} | {Position.Name}");

            if (Manager != null)
                toString.AppendLine($"Gerente: {Manager.Id} | {Manager.Name}");

            toString.AppendLine($"Direitos de Acesso: {SystemUserRoles.Count}");
            foreach (var role in SystemUserRoles)
                toString.AppendLine($"{role.Id} | {role.Name}");

            toString.AppendLine($"Equipes: {Teams.Count}");
            foreach (var team in Teams)
                toString.AppendLine($"{team.Id} | {team.Name} | {team.BusinessUnitId.Id}");

            toString.AppendLine($"Direitos de Acesso da Equipe: {TeamRoles.Count}");
            foreach (var role in TeamRoles)
                toString.AppendLine($"{role.Id} | {role.Name}");

            return toString.ToString();
        }
    }
}
