
using System;
using Newtonsoft.Json;

namespace Picker.Core.Models {
  public static class ModelData {

    public const string Key_Type = "type";
    /// <summary>
    /// 国家标准
    /// </summary>
    public const string Type_NationalStandard = "national_standard";
    /// <summary>
    /// 行业标准
    /// </summary>
    public const string Type_IndustryStandard = "industry_standard";

    public const string DocumentType_Json = "json";
    public const string DocumentType_Xml = "xml";
    public const string DocumentType_Html = "html";
    public const string DocumentType_Markdown = "markdown";

  }


  public class ModelTemplate {
    [JsonProperty( PropertyName = "id" )]
    public string Id { get; set; }

    [JsonProperty( PropertyName = "title" )]
    public string Title { get; set; }
    [JsonProperty( PropertyName = "summary" )]
    public string Summary { get; set; }

    //[JsonProperty( PropertyName = "content" )]
    //public string Content { get; set; }
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
    /// Content的字节数
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


    public ModelTemplate() {
      //Type = ModelData.Type_NationalStandard;
    }

  }

}
