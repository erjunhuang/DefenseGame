using Core.Utilities;
using System;

namespace Core.Utilities
{
    public abstract class ClassSingleton<T> where T : class, new()
    {
        private static T instance = null;
        public static T Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = new T();
                }
                return instance;
            }
        }
        protected ClassSingleton()
        {
            if (null != instance)
            {
                throw new SingletonException("This " +(typeof(T)).ToString() + " Singleton Instance is not null !!!");
            }

            Init();
        }
        public virtual void Init()
        {

        }
    }
}
