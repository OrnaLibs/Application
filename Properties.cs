using System;

namespace OrnaLibs.Application
{
    public partial struct Application
    {
        /// <summary>
        /// Название приложения
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Версия приложения
        /// </summary>
        public Version Version { get; internal set; }

        /// <summary>
        /// Отображаемое название приложения
        /// </summary>
        public string DisplayName { get; internal set; }

        /// <summary>
        /// Разработчик приложения (Организация в GitHub)
        /// </summary>
        public string CompanyName { get; internal set; }

        /// <summary>
        /// Путь до исполняемого файла
        /// </summary>
        public string AppPath { get; internal set; }

        /// <summary>
        /// Путь до иконки приложения
        /// </summary>
        public string IconPath { get; internal set; }

        /// <summary>
        /// Путь до конфигуратора
        /// </summary>
        public string Configurator { get; internal set; }

        /// <summary>
        /// Путь до деинсталятора
        /// </summary>
        public string Uninstaller { get; internal set; }

        /// <summary>
        /// Путь до сервис-приложения
        /// </summary>
        public string ServicePath { get; internal set; }
    }
}