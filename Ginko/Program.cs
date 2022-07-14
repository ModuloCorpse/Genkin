namespace Ginko
{
    internal static class Program
    {
        static CodeDrivenReader CreateCodeDrivenReader()
        {
            CodeDrivenReader codeDrivenReader = new();
            codeDrivenReader.AddAccount(35646.69, "Compte Mewen", timeLimit: new(2021, 1, 1));
            codeDrivenReader.AddMarker("Compte Mewen", new(2021, 1, 1), 35646.69);
            codeDrivenReader.AddMarker("Compte Mewen", new(2021, 2, 1), 35646.69);
            codeDrivenReader.AddMarker("Compte Mewen", new(2021, 3, 1), 35646.69);
            codeDrivenReader.AddMarker("Compte Mewen", new(2021, 4, 1), 35646.69);
            codeDrivenReader.AddMarker("Compte Mewen", new(2021, 5, 1), 35646.69);
            codeDrivenReader.AddMarker("Compte Mewen", new(2021, 6, 1), 5646.69);
            codeDrivenReader.AddSubAccount("Compte Mewen", 0, "Économie maison", "Économie pour la maison", new(2021, 1, 1));
            codeDrivenReader.AddMarker("Économie maison", new(2021, 1, 1), 0);
            codeDrivenReader.AddMarker("Économie maison", new(2021, 2, 1), 0);
            codeDrivenReader.AddMarker("Économie maison", new(2021, 3, 1), 0);
            codeDrivenReader.AddMarker("Économie maison", new(2021, 4, 1), 0);
            codeDrivenReader.AddMarker("Économie maison", new(2021, 5, 1), 0);
            codeDrivenReader.AddMarker("Économie maison", new(2021, 6, 1), 30000);
            codeDrivenReader.AddTransaction(30000, "Économie maison", "", new(2021, 5, 1), "Compte Mewen", "Économie maison");
            codeDrivenReader.AddTransaction(10, "Test1", "AC1", DateTime.Today, "Compte Mewen", "");
            codeDrivenReader.AddTransaction(10, "Test2", "AD2", DateTime.Today, "", "Compte Mewen");
            codeDrivenReader.AddTransaction(10, "Test3", "AC3", DateTime.Today, "Compte Mewen", "");
            codeDrivenReader.AddTransaction(10, "Test4", "BD4", DateTime.Today, "", "Compte Mewen");
            codeDrivenReader.AddTransaction(10, "Test5", "BC5", DateTime.Today, "Compte Mewen", "");
            codeDrivenReader.AddTransaction(10, "Test6", "BD6", DateTime.Today, "", "Compte Mewen");
            return codeDrivenReader;
        }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Database database = new();
            database.SetReader(CreateCodeDrivenReader());
            database.SetWriter(new CodeDrivenWriter());
            ApplicationConfiguration.Initialize();
            Application.Run(new GinkoForm(database));
            database.Write();
        }
    }
}