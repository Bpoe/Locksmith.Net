namespace Locksmith.Core
{
    using Etcdserverpb;
    using Google.Protobuf;
    using CompareResult = Etcdserverpb.Compare.Types.CompareResult;

    public static class TxnExtensions
    {
        public static Compare CompareModRevision(string key, CompareResult operation, long revision)
        {
            return new Compare
            {
                Key = ByteString.CopyFromUtf8(key),
                ModRevision = revision,
                Result = operation,
                Target = Compare.Types.CompareTarget.Mod,
            };
        }

        public static Compare CompareVersion(string key, CompareResult operation, long version)
        {
            return new Compare
            {
                Key = ByteString.CopyFromUtf8(key),
                Version = version,
                Result = operation,
                Target = Compare.Types.CompareTarget.Version,
            };
        }

        public static RequestOp Put(string key, string value)
        {
            return new RequestOp
            {
                RequestPut = new PutRequest
                {
                    Key = ByteString.CopyFromUtf8(key),
                    Value = ByteString.CopyFromUtf8(value),
                }
            };
        }
    }
}
