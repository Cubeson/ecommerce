using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Konscious.Security.Cryptography;

public class HashAlgorithms
{
    private readonly byte[] input = System.Text.Encoding.ASCII.GetBytes("SuperPass!1@2#3");

    [Benchmark]
    public string BenchmarkMD5()
    {
        using(var md5 = MD5.Create())
        {
            var ret = md5.ComputeHash(input);
            return Convert.ToHexString(ret);
        }
    }
    [Benchmark]
    public string BenchmarkSha256()
    {
        using(var sha256 = SHA256.Create())
        {
            var ret = sha256.ComputeHash(input);
            return Convert.ToHexString(ret);
        }
    }
    [Benchmark]
    public string BenchmarkArgon2()
    {
        var argon2 = new Argon2id(input)
        {
            MemorySize = 8192, //in KB -- To make hash cracking more expensive for an attacker, you want to make this value as high as possible
            DegreeOfParallelism = 16, // This should be chosen as high as possible to reduce the threat imposed by parallelized hash cracking
            Iterations = 1, // The execution time correlates linearly with this parameter. It allows you to increase the computational cost required to calculate one hash 
        };
        var ret = argon2.GetBytes(128);
        return Convert.ToHexString(ret);
    }
}
class Program
{
    static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<HashAlgorithms>();
    }
}