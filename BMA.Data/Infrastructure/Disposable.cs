using System;

namespace BMA.Data.Infrastructure
{
    class Disposable : IDisposable
    {
        private bool isDisposed;

        ~Disposable()
        {
            Dispose(false);
        }

        /// <summary>
        /// Base function of dispose class
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Base function of dispose class with alternative of dispose component
        /// </summary>
        /// <param name="disposing">true if you want dispose instance's component</param>
        private void Dispose(bool disposing)
        {
            if (isDisposed && disposing)
            {
                DisposeCore();
            }
            isDisposed = true;
        }

        /// <summary>
        /// Dispose function for custom instance (dispose instance's component)
        /// </summary>
        protected virtual void DisposeCore()
        {
        }
    }
}
