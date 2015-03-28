using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Picker.Core.Storage;

namespace Picker.Postgresql {
  public class StoreContext : IStorage {
    //NpgsqlConnection conn = null;
    DoubanEntities doubanContext = null;

    public StoreContext( string connString ) {
      OpenDatabase( connString );
    }

    public void OpenDatabase( string connString ) {
      //conn = new NpgsqlConnection( connString );
      doubanContext = new DoubanEntities( connString );
    }

    #region Interfaces

    public async Task<int> Douban_SaveBook( string url, string json ) {
      Book book = doubanContext.Book.Where( i => i.Uri == url ).FirstOrDefault();
      if ( book != null ) {
        book.Content = json;
        book.UpdatedAt = DateTime.UtcNow;
      }
      else {
        book = new Book();
        book.Content = json;
        book.CreatedAt = DateTime.UtcNow;
        book.UpdatedAt = DateTime.UtcNow;
        doubanContext.Book.Add( book );
      }
      return await doubanContext.SaveChangesAsync();
    }

    public async Task<int> Douban_DeleteBook( string url ) {
      Book book = doubanContext.Book.Where( i => i.Uri == url ).FirstOrDefault();
      if ( book != null ) {
        doubanContext.Book.Remove( book );
        return await doubanContext.SaveChangesAsync();
      }
      return 0;
    }

    public async Task<int> Douban_SaveMovie( string url, string json ) {
      //Book book = doubanContext.Book.Where( i => i.Uri == url ).FirstOrDefault();
      //if ( book != null ) {
      //  book.Content = json;
      //  book.UpdatedAt = DateTime.UtcNow;
      //}
      //else {
      //  book = new Book();
      //  book.Content = json;
      //  book.CreatedAt = DateTime.UtcNow;
      //  book.UpdatedAt = DateTime.UtcNow;
      //  doubanContext.Book.Add( book );
      //}
      return await doubanContext.SaveChangesAsync();
    }

    public async Task<int> Douban_DeleteMovie( string url ) {
      //Book book = doubanContext.Book.Where( i => i.Uri == url ).FirstOrDefault();
      //if ( book != null ) {
      //  doubanContext.Book.Remove( book );
      //  return await doubanContext.SaveChangesAsync();
      //}
      return 0;
    }

    #endregion Interfaces

  }

}
