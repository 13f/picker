using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picker.SqlServer {
  public partial class pickerEntities : System.Data.Entity.DbContext {
    public pickerEntities( string connectionString )
      : base( connectionString ) {
    }

  }
}
