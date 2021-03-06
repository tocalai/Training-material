//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/t4models).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------
using System;
using System.Linq;

using LinqToDB;
using LinqToDB.Mapping;

namespace DataModels
{
	/// <summary>
	/// Database       : kadai_dev
	/// Data Source    : 192.168.58.110
	/// Server Version : 9.1.19
	/// </summary>
	public partial class KadaiDevDB : LinqToDB.Data.DataConnection
	{
		public ITable<MtanakaApicreate>       MtanakaApicreate       { get { return this.GetTable<MtanakaApicreate>(); } }
		public ITable<MtanakaApiget>          MtanakaApiget          { get { return this.GetTable<MtanakaApiget>(); } }
		public ITable<MtanakaData>            MtanakaData            { get { return this.GetTable<MtanakaData>(); } }
		public ITable<MtanakaUsers>           MtanakaUsers           { get { return this.GetTable<MtanakaUsers>(); } }
		public ITable<Person>                 LaiData                { get { return this.GetTable<Person>(); } }
		public ITable<TaiApicreate>           TaiApicreate           { get { return this.GetTable<TaiApicreate>(); } }
		public ITable<TaiApiget>              TaiApiget              { get { return this.GetTable<TaiApiget>(); } }
		public ITable<TaiData>                TaiData                { get { return this.GetTable<TaiData>(); } }
		public ITable<TaiHelloworldweb>       TaiHelloworldweb       { get { return this.GetTable<TaiHelloworldweb>(); } }
		public ITable<TaiRecentlyvieweditems> TaiRecentlyvieweditems { get { return this.GetTable<TaiRecentlyvieweditems>(); } }
		public ITable<TestDataLin>            TestDataLin            { get { return this.GetTable<TestDataLin>(); } }
		public ITable<WeatherLin>             WeatherLin             { get { return this.GetTable<WeatherLin>(); } }

		public KadaiDevDB()
		{
			InitDataContext();
		}

		public KadaiDevDB(string configuration)
			: base(configuration)
		{
			InitDataContext();
		}

		partial void InitDataContext();
	}

	[Table(Schema="public", Name="mtanaka_apicreate")]
	public partial class MtanakaApicreate
	{
		[Column("line"),        NotNull] public string   Line        { get; set; } // character varying(64)
		[Column("date"),        NotNull] public DateTime Date        { get; set; } // date
		[Column("description"), NotNull] public string   Description { get; set; } // text
	}

	[Table(Schema="public", Name="mtanaka_apiget")]
	public partial class MtanakaApiget
	{
		[Column("id"),              PrimaryKey, Identity] public int      Id             { get; set; } // integer
		[Column("city_name"),       NotNull             ] public string   CityName       { get; set; } // character varying(64)
		[Column("date"),            NotNull             ] public DateTime Date           { get; set; } // date
		[Column("description"),     NotNull             ] public string   Description    { get; set; } // character varying(64)
		[Column("min_temperature"), NotNull             ] public int      MinTemperature { get; set; } // integer
		[Column("max_temperature"), NotNull             ] public int      MaxTemperature { get; set; } // integer
		[Column("intime"),          NotNull             ] public DateTime Intime         { get; set; } // date
		[Column("utime"),           NotNull             ] public DateTime Utime          { get; set; } // date
	}

	[Table(Schema="public", Name="mtanaka_data")]
	public partial class MtanakaData
	{
		[Column("id"),       PrimaryKey, Identity] public int      Id       { get; set; } // integer
		[Column("name"),     NotNull             ] public string   Name     { get; set; } // character varying(256)
		[Column("sex"),      NotNull             ] public int      Sex      { get; set; } // integer
		[Column("birthday"), NotNull             ] public DateTime Birthday { get; set; } // date
		[Column("intime"),   NotNull             ] public DateTime Intime   { get; set; } // date
		[Column("utime"),    NotNull             ] public DateTime Utime    { get; set; } // date
	}

	[Table(Schema="public", Name="mtanaka_users")]
	public partial class MtanakaUsers
	{
		[Column("id"),            PrimaryKey, NotNull] public string Id           { get; set; } // character varying(256)
		[Column("user_name"),                 NotNull] public string UserName     { get; set; } // character varying(256)
		[Column("password_hash"),             NotNull] public string PasswordHash { get; set; } // character varying(256)
		[Column("login_count"),               NotNull] public int    LoginCount   { get; set; } // integer
	}

	[Table(Schema="public", Name="lai_data")]
	public partial class Person
	{
		[Column("id"),       PrimaryKey, Identity] public int      Id       { get; set; } // integer
		[Column("name"),     NotNull             ] public string   Name     { get; set; } // character varying(512)
		[Column("sex"),      NotNull             ] public string   Sex      { get; set; } // character varying(5)
		[Column("birthday"), NotNull             ] public DateTime Birthday { get; set; } // date
		[Column("intime"),   NotNull             ] public DateTime Intime   { get; set; } // date
		[Column("utime"),    NotNull             ] public DateTime Utime    { get; set; } // date
	}

	[Table(Schema="public", Name="tai_apicreate")]
	public partial class TaiApicreate
	{
		[Column("id"),          PrimaryKey, Identity] public int       Id          { get; set; } // integer
		[Column("date"),        Nullable            ] public DateTime? Date        { get; set; } // date
		[Column("line"),        Nullable            ] public string    Line        { get; set; } // text
		[Column("description"), Nullable            ] public string    Description { get; set; } // text
	}

	[Table(Schema="public", Name="tai_apiget")]
	public partial class TaiApiget
	{
		[Column("id"),             PrimaryKey, Identity] public int       Id             { get; set; } // integer
		[Column("cityname"),       Nullable            ] public string    Cityname       { get; set; } // text
		[Column("description"),    Nullable            ] public string    Description    { get; set; } // text
		[Column("temperaturemax"), Nullable            ] public string    Temperaturemax { get; set; } // text
		[Column("temperaturemin"), Nullable            ] public string    Temperaturemin { get; set; } // text
		[Column("intime"),         Nullable            ] public DateTime? Intime         { get; set; } // timestamp (6) without time zone
	}

	[Table(Schema="public", Name="tai_data")]
	public partial class TaiData
	{
		[Column("id"),       PrimaryKey, Identity] public int       Id       { get; set; } // integer
		[Column("name"),     Nullable            ] public string    Name     { get; set; } // text
		[Column("sex"),      Nullable            ] public int?      Sex      { get; set; } // integer
		[Column("birthday"), Nullable            ] public DateTime? Birthday { get; set; } // timestamp (6) without time zone
		[Column("intime"),   Nullable            ] public DateTime? Intime   { get; set; } // timestamp (6) without time zone
		[Column("utime"),    Nullable            ] public DateTime? Utime    { get; set; } // timestamp (6) without time zone
	}

	[Table(Schema="public", Name="tai_helloworldweb")]
	public partial class TaiHelloworldweb
	{
		[Column("id"),         PrimaryKey, Identity] public int    Id         { get; set; } // integer
		[Column("username"),   Nullable            ] public string Username   { get; set; } // text
		[Column("password"),   Nullable            ] public string Password   { get; set; } // character varying
		[Column("logincount"), Nullable            ] public int?   Logincount { get; set; } // integer
	}

	[Table(Schema="public", Name="tai_recentlyvieweditems")]
	public partial class TaiRecentlyvieweditems
	{
		[Column("id"),          PrimaryKey, Identity] public int    Id          { get; set; } // integer
		[Column("item"),        Nullable            ] public string Item        { get; set; } // text
		[Column("description"), Nullable            ] public string Description { get; set; } // text
	}

	[Table(Schema="public", Name="test_data_lin")]
	public partial class TestDataLin
	{
		[Column("id"),          PrimaryKey,  Identity] public int             Id         { get; set; } // integer
		[Column("user_id"),     NotNull              ] public string          UserId     { get; set; } // text
		[Column("password"),    NotNull              ] public string          Password   { get; set; } // character varying(10)
		[Column("name"),        NotNull              ] public string          Name       { get; set; } // text
		[Column("sex"),         NotNull              ] public int             Sex        { get; set; } // integer
		[Column("birthday"),    NotNull              ] public DateTime        Birthday   { get; set; } // date
		[Column("intime"),         Nullable          ] public DateTimeOffset? Intime     { get; set; } // timestamp (6) with time zone
		[Column("login_count"), NotNull              ] public int             LoginCount { get; set; } // integer
		[Column("utime"),          Nullable          ] public DateTimeOffset? Utime      { get; set; } // timestamp (6) with time zone
	}

	[Table(Schema="public", Name="weather_lin")]
	public partial class WeatherLin
	{
		[Column("id"),          PrimaryKey, Identity] public int            Id          { get; set; } // integer
		[Column("cityname"),    NotNull             ] public string         Cityname    { get; set; } // text
		[Column("description"), NotNull             ] public string         Description { get; set; } // text
		[Column("temperature"), NotNull             ] public int            Temperature { get; set; } // integer
		[Column("intime"),      NotNull             ] public DateTimeOffset Intime      { get; set; } // timestamp (6) with time zone
	}

	public static partial class TableExtensions
	{
		public static MtanakaApiget Find(this ITable<MtanakaApiget> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static MtanakaData Find(this ITable<MtanakaData> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static MtanakaUsers Find(this ITable<MtanakaUsers> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Person Find(this ITable<Person> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static TaiApicreate Find(this ITable<TaiApicreate> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static TaiApiget Find(this ITable<TaiApiget> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static TaiData Find(this ITable<TaiData> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static TaiHelloworldweb Find(this ITable<TaiHelloworldweb> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static TaiRecentlyvieweditems Find(this ITable<TaiRecentlyvieweditems> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static TestDataLin Find(this ITable<TestDataLin> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static WeatherLin Find(this ITable<WeatherLin> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}
	}
}
