using System.Threading.Tasks;
using System;

namespace CrateBytes.Callbackable
{
    public static class Callbackable
    {
        public static async void asCallback<T>(this Task<T> task, Action<T> onComplete)
        {
            onComplete(await task);
        }
    }
}

