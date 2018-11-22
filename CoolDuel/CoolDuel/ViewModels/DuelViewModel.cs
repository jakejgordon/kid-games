using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolDuel.ViewModels
{
    public class DuelViewModel
    {
        public CharacterViewModel Character1 { get; set; }
        public CharacterViewModel Character2 { get; set; }

        public DuelViewModel()
        {
            Character1 = new CharacterViewModel();
            Character2 = new CharacterViewModel();
        }
    }
}
