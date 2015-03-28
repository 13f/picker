using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picker.Postgresql {
  public partial class DoubanEntities : System.Data.Entity.DbContext {
    public DoubanEntities( string connectionString )
      : base( connectionString ) {
    }

  }
}
