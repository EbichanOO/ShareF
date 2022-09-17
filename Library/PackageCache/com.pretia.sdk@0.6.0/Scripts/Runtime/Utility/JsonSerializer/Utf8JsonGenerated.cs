#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

namespace Utf8Json.Resolvers
{
    using System;
    using Utf8Json;

    public class GeneratedResolver : global::Utf8Json.IJsonFormatterResolver
    {
        public static readonly global::Utf8Json.IJsonFormatterResolver Instance = new GeneratedResolver();

        GeneratedResolver()
        {

        }

        public global::Utf8Json.IJsonFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly global::Utf8Json.IJsonFormatter<T> formatter;

            static FormatterCache()
            {
                var f = GeneratedResolverGetFormatterHelper.GetFormatter(typeof(T));
                if (f != null)
                {
                    formatter = (global::Utf8Json.IJsonFormatter<T>)f;
                }
            }
        }
    }

    internal static class GeneratedResolverGetFormatterHelper
    {
        static readonly global::System.Collections.Generic.Dictionary<Type, int> lookup;

        static GeneratedResolverGetFormatterHelper()
        {
            lookup = new global::System.Collections.Generic.Dictionary<Type, int>(18)
            {
                {typeof(global::System.Collections.Generic.List<string>), 0 },
                {typeof(global::PretiaArCloud.MapSelectionResponse.SelectedMap[]), 1 },
                {typeof(global::System.Guid[]), 2 },
                {typeof(global::PretiaArCloud.MapContent[]), 3 },
                {typeof(global::MapSelectionCriteria), 4 },
                {typeof(global::PretiaArCloud.MapGpsData), 5 },
                {typeof(global::PretiaArCloud.MapSelectionNetworkRequest), 6 },
                {typeof(global::PretiaArCloud.MapSelectionResponse.SelectedMap), 7 },
                {typeof(global::PretiaArCloud.MapSelectionResponse), 8 },
                {typeof(global::PretiaArCloud.AuthRequest), 9 },
                {typeof(global::PretiaArCloud.AuthResult), 10 },
                {typeof(global::PretiaArCloud.BaseClaims), 11 },
                {typeof(global::PretiaArCloud.EnduserClaims), 12 },
                {typeof(global::PretiaArCloud.Networking.UserData), 13 },
                {typeof(global::PretiaArCloud.ApiResult), 14 },
                {typeof(global::PretiaArCloud.MapContentTransform), 15 },
                {typeof(global::PretiaArCloud.MapContent), 16 },
                {typeof(global::PretiaArCloud.MapContentCollection), 17 },
            };
        }

        internal static object GetFormatter(Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key)) return null;

            switch (key)
            {
                case 0: return new global::Utf8Json.Formatters.ListFormatter<string>();
                case 1: return new global::Utf8Json.Formatters.ArrayFormatter<global::PretiaArCloud.MapSelectionResponse.SelectedMap>();
                case 2: return new global::Utf8Json.Formatters.ArrayFormatter<global::System.Guid>();
                case 3: return new global::Utf8Json.Formatters.ArrayFormatter<global::PretiaArCloud.MapContent>();
                case 4: return new Utf8Json.Formatters.MapSelectionCriteriaFormatter();
                case 5: return new Utf8Json.Formatters.PretiaArCloud.MapGpsDataFormatter();
                case 6: return new Utf8Json.Formatters.PretiaArCloud.MapSelectionNetworkRequestFormatter();
                case 7: return new Utf8Json.Formatters.PretiaArCloud.MapSelectionResponse_SelectedMapFormatter();
                case 8: return new Utf8Json.Formatters.PretiaArCloud.MapSelectionResponseFormatter();
                case 9: return new Utf8Json.Formatters.PretiaArCloud.AuthRequestFormatter();
                case 10: return new Utf8Json.Formatters.PretiaArCloud.AuthResultFormatter();
                case 11: return new Utf8Json.Formatters.PretiaArCloud.BaseClaimsFormatter();
                case 12: return new Utf8Json.Formatters.PretiaArCloud.EnduserClaimsFormatter();
                case 13: return new Utf8Json.Formatters.PretiaArCloud.Networking.UserDataFormatter();
                case 14: return new Utf8Json.Formatters.PretiaArCloud.ApiResultFormatter();
                case 15: return new Utf8Json.Formatters.PretiaArCloud.MapContentTransformFormatter();
                case 16: return new Utf8Json.Formatters.PretiaArCloud.MapContentFormatter();
                case 17: return new Utf8Json.Formatters.PretiaArCloud.MapContentCollectionFormatter();
                default: return null;
            }
        }
    }
}

#pragma warning disable 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 168

namespace Utf8Json.Formatters
{
    using System;
    using Utf8Json;


    public sealed class MapSelectionCriteriaFormatter : global::Utf8Json.IJsonFormatter<global::MapSelectionCriteria>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public MapSelectionCriteriaFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Public"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Merged"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("GroupKeys"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("GpsThreshold"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("TimeThreshold"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Sorting"), 5},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("Public"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Merged"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("GroupKeys"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("GpsThreshold"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("TimeThreshold"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Sorting"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::MapSelectionCriteria value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::MapSelectionCriteria.PublicStatus>().Serialize(ref writer, value.Public, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::MapSelectionCriteria.MergeStatus>().Serialize(ref writer, value.Merged, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Serialize(ref writer, value.GroupKeys, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteSingle(value.GpsThreshold);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteSingle(value.TimeThreshold);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<global::MapSelectionCriteria.ResultOrder>().Serialize(ref writer, value.Sorting, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::MapSelectionCriteria Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __Public__ = default(global::MapSelectionCriteria.PublicStatus);
            var __Public__b__ = false;
            var __Merged__ = default(global::MapSelectionCriteria.MergeStatus);
            var __Merged__b__ = false;
            var __GroupKeys__ = default(global::System.Collections.Generic.List<string>);
            var __GroupKeys__b__ = false;
            var __GpsThreshold__ = default(float);
            var __GpsThreshold__b__ = false;
            var __TimeThreshold__ = default(float);
            var __TimeThreshold__b__ = false;
            var __Sorting__ = default(global::MapSelectionCriteria.ResultOrder);
            var __Sorting__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __Public__ = formatterResolver.GetFormatterWithVerify<global::MapSelectionCriteria.PublicStatus>().Deserialize(ref reader, formatterResolver);
                        __Public__b__ = true;
                        break;
                    case 1:
                        __Merged__ = formatterResolver.GetFormatterWithVerify<global::MapSelectionCriteria.MergeStatus>().Deserialize(ref reader, formatterResolver);
                        __Merged__b__ = true;
                        break;
                    case 2:
                        __GroupKeys__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Deserialize(ref reader, formatterResolver);
                        __GroupKeys__b__ = true;
                        break;
                    case 3:
                        __GpsThreshold__ = reader.ReadSingle();
                        __GpsThreshold__b__ = true;
                        break;
                    case 4:
                        __TimeThreshold__ = reader.ReadSingle();
                        __TimeThreshold__b__ = true;
                        break;
                    case 5:
                        __Sorting__ = formatterResolver.GetFormatterWithVerify<global::MapSelectionCriteria.ResultOrder>().Deserialize(ref reader, formatterResolver);
                        __Sorting__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::MapSelectionCriteria();
            if(__Public__b__) ____result.Public = __Public__;
            if(__Merged__b__) ____result.Merged = __Merged__;
            if(__GroupKeys__b__) ____result.GroupKeys = __GroupKeys__;
            if(__GpsThreshold__b__) ____result.GpsThreshold = __GpsThreshold__;
            if(__TimeThreshold__b__) ____result.TimeThreshold = __TimeThreshold__;
            if(__Sorting__b__) ____result.Sorting = __Sorting__;

            return ____result;
        }
    }

}

#pragma warning disable 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 168

namespace Utf8Json.Formatters.PretiaArCloud
{
    using System;
    using Utf8Json;


    public sealed class MapGpsDataFormatter : global::Utf8Json.IJsonFormatter<global::PretiaArCloud.MapGpsData>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public MapGpsDataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("lat"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("long"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("alt"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("bear"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("acc"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("vacc"), 5},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("lat"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("long"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("alt"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("bear"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("acc"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("vacc"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::PretiaArCloud.MapGpsData value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteDouble(value.Latitude);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteDouble(value.Longitude);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteDouble(value.Altitude);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteSingle(value.Bearing);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteSingle(value.Accuracy);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteSingle(value.VerticalAccuracy);
            
            writer.WriteEndObject();
        }

        public global::PretiaArCloud.MapGpsData Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __Latitude__ = default(double);
            var __Latitude__b__ = false;
            var __Longitude__ = default(double);
            var __Longitude__b__ = false;
            var __Altitude__ = default(double);
            var __Altitude__b__ = false;
            var __Bearing__ = default(float);
            var __Bearing__b__ = false;
            var __Accuracy__ = default(float);
            var __Accuracy__b__ = false;
            var __VerticalAccuracy__ = default(float);
            var __VerticalAccuracy__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __Latitude__ = reader.ReadDouble();
                        __Latitude__b__ = true;
                        break;
                    case 1:
                        __Longitude__ = reader.ReadDouble();
                        __Longitude__b__ = true;
                        break;
                    case 2:
                        __Altitude__ = reader.ReadDouble();
                        __Altitude__b__ = true;
                        break;
                    case 3:
                        __Bearing__ = reader.ReadSingle();
                        __Bearing__b__ = true;
                        break;
                    case 4:
                        __Accuracy__ = reader.ReadSingle();
                        __Accuracy__b__ = true;
                        break;
                    case 5:
                        __VerticalAccuracy__ = reader.ReadSingle();
                        __VerticalAccuracy__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::PretiaArCloud.MapGpsData();

            return ____result;
        }
    }


    public sealed class MapSelectionNetworkRequestFormatter : global::Utf8Json.IJsonFormatter<global::PretiaArCloud.MapSelectionNetworkRequest>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public MapSelectionNetworkRequestFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("private_yn"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("public_yn"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("groups"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("merge_status"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("gps"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("gps_threshold"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time_env"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time_threshold"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sort_key"), 8},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("private_yn"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("public_yn"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("groups"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("merge_status"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("gps"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("gps_threshold"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("time_env"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("time_threshold"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("sort_key"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::PretiaArCloud.MapSelectionNetworkRequest value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteBoolean(value.OnlyPrivateMaps);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteBoolean(value.OnlyPublicMaps);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<string[]>().Serialize(ref writer, value.Groups, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.MergeStatus);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.MapGpsData>().Serialize(ref writer, value.Gps, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteSingle(value.GpsThreshold);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.TimeEnv);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteSingle(value.TimeThreshold);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteString(value.SortKey);
            
            writer.WriteEndObject();
        }

        public global::PretiaArCloud.MapSelectionNetworkRequest Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __OnlyPrivateMaps__ = default(bool);
            var __OnlyPrivateMaps__b__ = false;
            var __OnlyPublicMaps__ = default(bool);
            var __OnlyPublicMaps__b__ = false;
            var __Groups__ = default(string[]);
            var __Groups__b__ = false;
            var __MergeStatus__ = default(string);
            var __MergeStatus__b__ = false;
            var __Gps__ = default(global::PretiaArCloud.MapGpsData);
            var __Gps__b__ = false;
            var __GpsThreshold__ = default(float);
            var __GpsThreshold__b__ = false;
            var __TimeEnv__ = default(string);
            var __TimeEnv__b__ = false;
            var __TimeThreshold__ = default(float);
            var __TimeThreshold__b__ = false;
            var __SortKey__ = default(string);
            var __SortKey__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __OnlyPrivateMaps__ = reader.ReadBoolean();
                        __OnlyPrivateMaps__b__ = true;
                        break;
                    case 1:
                        __OnlyPublicMaps__ = reader.ReadBoolean();
                        __OnlyPublicMaps__b__ = true;
                        break;
                    case 2:
                        __Groups__ = formatterResolver.GetFormatterWithVerify<string[]>().Deserialize(ref reader, formatterResolver);
                        __Groups__b__ = true;
                        break;
                    case 3:
                        __MergeStatus__ = reader.ReadString();
                        __MergeStatus__b__ = true;
                        break;
                    case 4:
                        __Gps__ = formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.MapGpsData>().Deserialize(ref reader, formatterResolver);
                        __Gps__b__ = true;
                        break;
                    case 5:
                        __GpsThreshold__ = reader.ReadSingle();
                        __GpsThreshold__b__ = true;
                        break;
                    case 6:
                        __TimeEnv__ = reader.ReadString();
                        __TimeEnv__b__ = true;
                        break;
                    case 7:
                        __TimeThreshold__ = reader.ReadSingle();
                        __TimeThreshold__b__ = true;
                        break;
                    case 8:
                        __SortKey__ = reader.ReadString();
                        __SortKey__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::PretiaArCloud.MapSelectionNetworkRequest();
            if(__OnlyPrivateMaps__b__) ____result.OnlyPrivateMaps = __OnlyPrivateMaps__;
            if(__OnlyPublicMaps__b__) ____result.OnlyPublicMaps = __OnlyPublicMaps__;
            if(__Groups__b__) ____result.Groups = __Groups__;
            if(__MergeStatus__b__) ____result.MergeStatus = __MergeStatus__;
            if(__Gps__b__) ____result.Gps = __Gps__;
            if(__GpsThreshold__b__) ____result.GpsThreshold = __GpsThreshold__;
            if(__TimeEnv__b__) ____result.TimeEnv = __TimeEnv__;
            if(__TimeThreshold__b__) ____result.TimeThreshold = __TimeThreshold__;
            if(__SortKey__b__) ____result.SortKey = __SortKey__;

            return ____result;
        }
    }


    public sealed class MapSelectionResponse_SelectedMapFormatter : global::Utf8Json.IJsonFormatter<global::PretiaArCloud.MapSelectionResponse.SelectedMap>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public MapSelectionResponse_SelectedMapFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("gps_distance"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time_distance"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("map_key"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("gps_distance"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("time_distance"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("map_key"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::PretiaArCloud.MapSelectionResponse.SelectedMap value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.GpsDistance);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteSingle(value.TimeDistance);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.MapKey);
            
            writer.WriteEndObject();
        }

        public global::PretiaArCloud.MapSelectionResponse.SelectedMap Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            

            var __GpsDistance__ = default(float);
            var __GpsDistance__b__ = false;
            var __TimeDistance__ = default(float);
            var __TimeDistance__b__ = false;
            var __MapKey__ = default(string);
            var __MapKey__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __GpsDistance__ = reader.ReadSingle();
                        __GpsDistance__b__ = true;
                        break;
                    case 1:
                        __TimeDistance__ = reader.ReadSingle();
                        __TimeDistance__b__ = true;
                        break;
                    case 2:
                        __MapKey__ = reader.ReadString();
                        __MapKey__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::PretiaArCloud.MapSelectionResponse.SelectedMap();
            if(__GpsDistance__b__) ____result.GpsDistance = __GpsDistance__;
            if(__TimeDistance__b__) ____result.TimeDistance = __TimeDistance__;
            if(__MapKey__b__) ____result.MapKey = __MapKey__;

            return ____result;
        }
    }


    public sealed class MapSelectionResponseFormatter : global::Utf8Json.IJsonFormatter<global::PretiaArCloud.MapSelectionResponse>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public MapSelectionResponseFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("maps"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("error"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("error_code"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("maps"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("error"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("error_code"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::PretiaArCloud.MapSelectionResponse value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.MapSelectionResponse.SelectedMap[]>().Serialize(ref writer, value.SelectedMaps, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.Message);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteInt32(value.StatusCode);
            
            writer.WriteEndObject();
        }

        public global::PretiaArCloud.MapSelectionResponse Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __SelectedMaps__ = default(global::PretiaArCloud.MapSelectionResponse.SelectedMap[]);
            var __SelectedMaps__b__ = false;
            var __Message__ = default(string);
            var __Message__b__ = false;
            var __StatusCode__ = default(int);
            var __StatusCode__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __SelectedMaps__ = formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.MapSelectionResponse.SelectedMap[]>().Deserialize(ref reader, formatterResolver);
                        __SelectedMaps__b__ = true;
                        break;
                    case 1:
                        __Message__ = reader.ReadString();
                        __Message__b__ = true;
                        break;
                    case 2:
                        __StatusCode__ = reader.ReadInt32();
                        __StatusCode__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::PretiaArCloud.MapSelectionResponse();
            if(__SelectedMaps__b__) ____result.SelectedMaps = __SelectedMaps__;
            if(__Message__b__) ____result.Message = __Message__;
            if(__StatusCode__b__) ____result.StatusCode = __StatusCode__;

            return ____result;
        }
    }


    public sealed class AuthRequestFormatter : global::Utf8Json.IJsonFormatter<global::PretiaArCloud.AuthRequest>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AuthRequestFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("user_name"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("password"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("user_name"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("password"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::PretiaArCloud.AuthRequest value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.Username);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.Password);
            
            writer.WriteEndObject();
        }

        public global::PretiaArCloud.AuthRequest Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __Username__ = default(string);
            var __Username__b__ = false;
            var __Password__ = default(string);
            var __Password__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __Username__ = reader.ReadString();
                        __Username__b__ = true;
                        break;
                    case 1:
                        __Password__ = reader.ReadString();
                        __Password__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::PretiaArCloud.AuthRequest();
            if(__Username__b__) ____result.Username = __Username__;
            if(__Password__b__) ____result.Password = __Password__;

            return ____result;
        }
    }


    public sealed class AuthResultFormatter : global::Utf8Json.IJsonFormatter<global::PretiaArCloud.AuthResult>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AuthResultFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("token"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("display_name"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("error"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("error_code"), 3},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("token"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("display_name"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("error"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("error_code"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::PretiaArCloud.AuthResult value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.Token);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.DisplayName);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.Message);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteInt32(value.StatusCode);
            
            writer.WriteEndObject();
        }

        public global::PretiaArCloud.AuthResult Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __Token__ = default(string);
            var __Token__b__ = false;
            var __DisplayName__ = default(string);
            var __DisplayName__b__ = false;
            var __Message__ = default(string);
            var __Message__b__ = false;
            var __StatusCode__ = default(int);
            var __StatusCode__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __Token__ = reader.ReadString();
                        __Token__b__ = true;
                        break;
                    case 1:
                        __DisplayName__ = reader.ReadString();
                        __DisplayName__b__ = true;
                        break;
                    case 2:
                        __Message__ = reader.ReadString();
                        __Message__b__ = true;
                        break;
                    case 3:
                        __StatusCode__ = reader.ReadInt32();
                        __StatusCode__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::PretiaArCloud.AuthResult();
            if(__Token__b__) ____result.Token = __Token__;
            if(__DisplayName__b__) ____result.DisplayName = __DisplayName__;
            if(__Message__b__) ____result.Message = __Message__;
            if(__StatusCode__b__) ____result.StatusCode = __StatusCode__;

            return ____result;
        }
    }


    public sealed class BaseClaimsFormatter : global::Utf8Json.IJsonFormatter<global::PretiaArCloud.BaseClaims>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public BaseClaimsFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("exp"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("iat"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("exp"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("iat"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::PretiaArCloud.BaseClaims value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt64(value.ExpiredAt);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt64(value.IssuedAt);
            
            writer.WriteEndObject();
        }

        public global::PretiaArCloud.BaseClaims Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __ExpiredAt__ = default(long);
            var __ExpiredAt__b__ = false;
            var __IssuedAt__ = default(long);
            var __IssuedAt__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __ExpiredAt__ = reader.ReadInt64();
                        __ExpiredAt__b__ = true;
                        break;
                    case 1:
                        __IssuedAt__ = reader.ReadInt64();
                        __IssuedAt__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::PretiaArCloud.BaseClaims();
            if(__ExpiredAt__b__) ____result.ExpiredAt = __ExpiredAt__;
            if(__IssuedAt__b__) ____result.IssuedAt = __IssuedAt__;

            return ____result;
        }
    }


    public sealed class EnduserClaimsFormatter : global::Utf8Json.IJsonFormatter<global::PretiaArCloud.EnduserClaims>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public EnduserClaimsFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("payload"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("exp"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("iat"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("payload"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("exp"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("iat"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::PretiaArCloud.EnduserClaims value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.Payload);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt64(value.ExpiredAt);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteInt64(value.IssuedAt);
            
            writer.WriteEndObject();
        }

        public global::PretiaArCloud.EnduserClaims Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __Payload__ = default(string);
            var __Payload__b__ = false;
            var __ExpiredAt__ = default(long);
            var __ExpiredAt__b__ = false;
            var __IssuedAt__ = default(long);
            var __IssuedAt__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __Payload__ = reader.ReadString();
                        __Payload__b__ = true;
                        break;
                    case 1:
                        __ExpiredAt__ = reader.ReadInt64();
                        __ExpiredAt__b__ = true;
                        break;
                    case 2:
                        __IssuedAt__ = reader.ReadInt64();
                        __IssuedAt__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::PretiaArCloud.EnduserClaims();
            if(__Payload__b__) ____result.Payload = __Payload__;
            if(__ExpiredAt__b__) ____result.ExpiredAt = __ExpiredAt__;
            if(__IssuedAt__b__) ____result.IssuedAt = __IssuedAt__;

            return ____result;
        }
    }


    public sealed class ApiResultFormatter : global::Utf8Json.IJsonFormatter<global::PretiaArCloud.ApiResult>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ApiResultFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("error"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("error_code"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("error"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("error_code"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::PretiaArCloud.ApiResult value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.Message);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt32(value.StatusCode);
            
            writer.WriteEndObject();
        }

        public global::PretiaArCloud.ApiResult Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __Message__ = default(string);
            var __Message__b__ = false;
            var __StatusCode__ = default(int);
            var __StatusCode__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __Message__ = reader.ReadString();
                        __Message__b__ = true;
                        break;
                    case 1:
                        __StatusCode__ = reader.ReadInt32();
                        __StatusCode__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::PretiaArCloud.ApiResult();
            if(__Message__b__) ____result.Message = __Message__;
            if(__StatusCode__b__) ____result.StatusCode = __StatusCode__;

            return ____result;
        }
    }


    public sealed class MapContentTransformFormatter : global::Utf8Json.IJsonFormatter<global::PretiaArCloud.MapContentTransform>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public MapContentTransformFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("pos"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("rot"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("scale"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("pos"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("rot"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("scale"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::PretiaArCloud.MapContentTransform value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<UnityEngine.Vector3>().Serialize(ref writer, value.Position, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<UnityEngine.Quaternion>().Serialize(ref writer, value.Rotation, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<UnityEngine.Vector3>().Serialize(ref writer, value.Scale, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::PretiaArCloud.MapContentTransform Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __Position__ = default(UnityEngine.Vector3);
            var __Position__b__ = false;
            var __Rotation__ = default(UnityEngine.Quaternion);
            var __Rotation__b__ = false;
            var __Scale__ = default(UnityEngine.Vector3);
            var __Scale__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __Position__ = formatterResolver.GetFormatterWithVerify<UnityEngine.Vector3>().Deserialize(ref reader, formatterResolver);
                        __Position__b__ = true;
                        break;
                    case 1:
                        __Rotation__ = formatterResolver.GetFormatterWithVerify<UnityEngine.Quaternion>().Deserialize(ref reader, formatterResolver);
                        __Rotation__b__ = true;
                        break;
                    case 2:
                        __Scale__ = formatterResolver.GetFormatterWithVerify<UnityEngine.Vector3>().Deserialize(ref reader, formatterResolver);
                        __Scale__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::PretiaArCloud.MapContentTransform();
            if(__Position__b__) ____result.Position = __Position__;
            if(__Rotation__b__) ____result.Rotation = __Rotation__;
            if(__Scale__b__) ____result.Scale = __Scale__;

            return ____result;
        }
    }


    public sealed class MapContentFormatter : global::Utf8Json.IJsonFormatter<global::PretiaArCloud.MapContent>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public MapContentFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("id"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("name"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("asset_path"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("transform"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("children"), 4},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("id"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("name"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("asset_path"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("transform"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("children"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::PretiaArCloud.MapContent value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.Guid>().Serialize(ref writer, value.Id, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.Name);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.AssetPath);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.MapContentTransform>().Serialize(ref writer, value.Transform, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<global::System.Guid[]>().Serialize(ref writer, value.Children, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::PretiaArCloud.MapContent Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __Id__ = default(global::System.Guid);
            var __Id__b__ = false;
            var __Name__ = default(string);
            var __Name__b__ = false;
            var __AssetPath__ = default(string);
            var __AssetPath__b__ = false;
            var __Transform__ = default(global::PretiaArCloud.MapContentTransform);
            var __Transform__b__ = false;
            var __Children__ = default(global::System.Guid[]);
            var __Children__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __Id__ = formatterResolver.GetFormatterWithVerify<global::System.Guid>().Deserialize(ref reader, formatterResolver);
                        __Id__b__ = true;
                        break;
                    case 1:
                        __Name__ = reader.ReadString();
                        __Name__b__ = true;
                        break;
                    case 2:
                        __AssetPath__ = reader.ReadString();
                        __AssetPath__b__ = true;
                        break;
                    case 3:
                        __Transform__ = formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.MapContentTransform>().Deserialize(ref reader, formatterResolver);
                        __Transform__b__ = true;
                        break;
                    case 4:
                        __Children__ = formatterResolver.GetFormatterWithVerify<global::System.Guid[]>().Deserialize(ref reader, formatterResolver);
                        __Children__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::PretiaArCloud.MapContent();
            if(__Id__b__) ____result.Id = __Id__;
            if(__Name__b__) ____result.Name = __Name__;
            if(__AssetPath__b__) ____result.AssetPath = __AssetPath__;
            if(__Transform__b__) ____result.Transform = __Transform__;
            if(__Children__b__) ____result.Children = __Children__;

            return ____result;
        }
    }


    public sealed class MapContentCollectionFormatter : global::Utf8Json.IJsonFormatter<global::PretiaArCloud.MapContentCollection>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public MapContentCollectionFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("contents"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("contents"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::PretiaArCloud.MapContentCollection value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.MapContent[]>().Serialize(ref writer, value.Contents, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::PretiaArCloud.MapContentCollection Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __Contents__ = default(global::PretiaArCloud.MapContent[]);
            var __Contents__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __Contents__ = formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.MapContent[]>().Deserialize(ref reader, formatterResolver);
                        __Contents__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::PretiaArCloud.MapContentCollection();
            if(__Contents__b__) ____result.Contents = __Contents__;

            return ____result;
        }
    }

}

#pragma warning disable 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 168

namespace Utf8Json.Formatters.PretiaArCloud.Networking
{
    using System;
    using Utf8Json;


    public sealed class UserDataFormatter : global::Utf8Json.IJsonFormatter<global::PretiaArCloud.Networking.UserData>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public UserDataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Token"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("DisplayName"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("Token"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("DisplayName"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::PretiaArCloud.Networking.UserData value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.Token);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.DisplayName);
            
            writer.WriteEndObject();
        }

        public global::PretiaArCloud.Networking.UserData Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            
            
	        throw new InvalidOperationException("generated serializer for IInterface does not support deserialize.");
        }
    }

}

#pragma warning disable 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
