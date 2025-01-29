using System;

namespace OrnaLibs.Application
{

    /// <summary>
    /// Информация о приложении
    /// </summary>
    public partial struct Application
    {
        /// <summary>
        /// Регистрация приложения
        /// </summary>
        public void Registration()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                RegistrationWindows();
        }

        /// <summary>
        /// Удаление записи о приложении
        /// </summary>
        public void Unregistration()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                UnregistrationWindows();
        }

        /// <summary>
        /// Регистрация сервиса
        /// </summary>
        public void RegistrationService(bool needThrow)
        {
            if (!ExistsServicePath(needThrow)) return;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                CreateWindowsService();
        }

        /// <summary>
        /// Удаление сервиса
        /// </summary>
        public void UnregistrationService(bool needThrow)
        {
            if (!ExistsServicePath(needThrow)) return;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                DeleteWindowsService();
        }

        private bool ExistsServicePath(bool needThrow)
        {
            if (!string.IsNullOrWhiteSpace(ServicePath)) return true;
            if (needThrow)
                throw new ArgumentNullException(nameof(ServicePath));
            return false;
        }
    }
}