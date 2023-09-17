using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyCiv.Shared.Dto
{
    public class PlayerDto
    {
        public int Id { get; private set; }

        public PlayerDto(int id)
        {
            Id = id;
        }
    }
}
