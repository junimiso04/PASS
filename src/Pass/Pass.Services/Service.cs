using System;
using System.Collections.Generic;
using System.Text;

namespace Pass.Services
{
    /// <summary>
    /// Provides a mechanism for managing your services.
    /// </summary>
    public abstract class Service : IDisposable
    {
        /// <summary>
        /// The name of the service to be registered in the service list.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The path to an external executable file that provides the service.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Provides the process ID for the current service. If the service is not running, returns '-1'.
        /// </summary>
        public int Pid { get; set; }

        /// <summary>
        /// The current status of the service.
        /// </summary>
        public ServiceStatus Status { get; set; }

        /// <summary>
        /// Starts the service.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown when the file path is not entered.</exception>
        /// <exception cref="System.ComponentModel.Win32Exception">Thrown when an unexecutable file has been entered or access to the file is not possible.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when service is already disposed.</exception>
        /// <exception cref="System.PlatformNotSupportedException">Thrown when the method is run on a Linux or macOS.</exception>
        public virtual void Start()
        {

        }

        /// <summary>
        /// Processes the remaining tasks and stops the service.
        /// </summary>
        public virtual void Stop()
        {

        }


        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리형 상태(관리형 개체)를 삭제합니다.
                }

                // TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.
                _disposedValue = true;
            }
        }

        // // TODO: 비관리형 리소스를 해제하는 코드가 'Dispose(bool disposing)'에 포함된 경우에만 종료자를 재정의합니다.
        // ~CompilerService()
        // {
        //     // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
