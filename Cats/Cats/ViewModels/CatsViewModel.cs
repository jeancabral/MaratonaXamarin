using Cats.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Cats.ViewModels
{
    public class CatsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool Busy;
        public ObservableCollection<Cat> Cats { get; set; }

        private void OnPropertyChanged(
            [System.Runtime.CompilerServices.CallerMemberName]
            string propertyName = null) => 
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs(propertyName));

        public bool IsBusy
        {
            get
            {
                return Busy;
            }
            set
            {
                Busy = value;
                OnPropertyChanged();

                //o seguinte código ao final do bloco set da propriedade IsBusy invoca o
                //método ChangeCanExecute do comando GetCatsCommand. Ao executar o método
                //ChangeCanExecute, a função que determina se o comando está habilitado será reavaliada.
                GetCatsCommand.ChangeCanExecute();
            }
        }

        public CatsViewModel()
        {
            Cats = new ObservableCollection<Models.Cat>();


            //Código para inicializar o
            //comando GetCatsCommand passando a ele dois métodos: um para se invocar quando o
            //comando for executado e outro para determinar quando o comando estiver habilitado.
            GetCatsCommand = new Command(
                async () => await GetCats(),
                () => !IsBusy
                );

        }

        async Task GetCats()
        {
            if (!IsBusy)
            {
                Exception Error = null;
                try
                {
                    IsBusy = true;
                    var Repository = new Repository();
                    var Items = await Repository.GetCats();

                    Cats.Clear();
                    foreach (var Cat in Items)
                    {
                        Cats.Add(Cat);
                    }
                }
                catch (Exception ex)
                {
                    Error = ex;
                }
                finally
                {
                    IsBusy = false;
                }

                //mostrar uma mensagem em caso de que se tenha gerado uma exceção.
                if (Error != null)
                {
                    await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                    "Error!", Error.Message, "OK");
                }
            }
            return;
        }



        public Command GetCatsCommand { get; set; }


    }


}
