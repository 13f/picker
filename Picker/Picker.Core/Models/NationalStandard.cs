
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Picker.Core.Models {
  public class NationalStandard {
    [JsonProperty( PropertyName = "id" )]
    public string Id { get; set; }

    [JsonProperty( PropertyName = "chinese_title" )]
    public string ChineseTitle { get; set; }
    [JsonProperty( PropertyName = "english_title" )]
    public string EnglishTitle { get; set; }
    [JsonProperty( PropertyName = "remark" )]
    public string Remark { get; set; }
    [JsonProperty( PropertyName = "code" )]
    public string Code { get; set; }
    [JsonProperty( PropertyName = "ics" )]
    public string ICS { get; set; }
    [JsonProperty( PropertyName = "ccs" )]
    public string CCS { get; set; }

    [JsonProperty( PropertyName = "area_scope" )]
    public string AreaScope { get; set; }

    [JsonProperty( PropertyName = "issuance_date" )]
    public DateTime? IssuanceDate { get; set; }
    [JsonProperty( PropertyName = "execute_date" )]
    public DateTime? ExecuteDate { get; set; }
    [JsonProperty( PropertyName = "revocatory_date" )]
    public DateTime? RevocatoryDate { get; set; }

    [JsonProperty( PropertyName = "token" )]
    public Newtonsoft.Json.Linq.JToken Token { get; set; }

    [JsonProperty( PropertyName = "type" )]
    public string Type { get; set; }
    ///// <summary>
    ///// {domain}
    ///// </summary>
    //[JsonProperty( PropertyName = "contentType" )]
    //public string ContentType { get; set; }
    /// <summary>
    /// Content(Token.ToString)的字节数
    /// </summary>
    [JsonProperty( PropertyName = "bytesCount" )]
    public int BytesCount { get; set; }

    [JsonProperty( PropertyName = "version" )]
    public int Version { get; set; }
    [JsonProperty( PropertyName = "versionId" )]
    public string VersionId { get; set; }

    [JsonProperty( PropertyName = "created" )]
    public DateTime Created { get; set; }
    [JsonProperty( PropertyName = "modified" )]
    public DateTime Modified { get; set; }


    public NationalStandard() {
      Type = ModelData.Type_NationalStandard;
    }
    
  }

}
