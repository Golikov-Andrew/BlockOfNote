using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;

namespace BlockOfNote.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void MenuItem_OpenFile_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Открытие файла",
            AllowMultiple = false
        });

        if (files.Count == 1)
        {
            await using var stream = await files[0].OpenReadAsync();
            using var streamReader = new StreamReader(stream);
            MainTextContent.Text = await streamReader.ReadToEndAsync();
            PathToFile.Text = files[0].Path.ToString();
            RefreshInfo();
        }
    }

    private void MenuItem_SaveFile_OnClick(object? sender, RoutedEventArgs e)
    {
        if (PathToFile.Text != "" && PathToFile.Text != null)
        {
            string correctFilePath = PathToFile.Text.Replace("file://", "");
            File.WriteAllText(correctFilePath, MainTextContent.Text);
        }
        else
        {
            SaveFileProcedure();
        }
        
    }
    
    private async void SaveFileProcedure ()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Сохранить файл как"
        });

        if (file is not null)
        {
            await using var stream = await file.OpenWriteAsync();
            using var streamWriter = new StreamWriter(stream);
            await streamWriter.WriteLineAsync(MainTextContent.Text);
            PathToFile.Text = file.Path.ToString();
        }
    }

    private async void MenuItem_SaveAsFile_OnClick(object? sender, RoutedEventArgs e)
    {
        SaveFileProcedure();
    }

    private void MenuItem_CreateFile_OnClick(object? sender, RoutedEventArgs e)
    {
        // Не совсем правильно, надо спрашивать пользователя "Сохранить изменения перед созданием нового файла?" 
        PathToFile.Text = "";
        MainTextContent.Text = "";
    }

    private void MenuItem_ChangeFontFamily_OnClick(object? sender, RoutedEventArgs e)
    {
        MainTextContent.FontFamily = new FontFamily((string)((Avalonia.Controls.MenuItem) sender).Header);
    }
    private void MenuItem_ChangeFontWeight_OnClick(object? sender, RoutedEventArgs e)
    {
        string menuHeaderValue = (string)((Avalonia.Controls.MenuItem)sender).Header;
        switch (menuHeaderValue)
        {
            case "Light":
            {
                MainTextContent.FontWeight = FontWeight.Light;
                break;
            }
            case "Normal":
            {
                MainTextContent.FontWeight = FontWeight.Normal;
                break;
            }
            case "Bold":
            {
                MainTextContent.FontWeight = FontWeight.Bold;
                break;
            }
        }
        
    }

    private void MenuItem_ChangeFontSize_OnClick(object? sender, RoutedEventArgs e)
    {
        MainTextContent.FontSize = Double.Parse((string)((Avalonia.Controls.MenuItem)sender).Header);
    }

    private void MenuItem_ChangeWordWrap_OnClick(object? sender, RoutedEventArgs e)
    {
        MainTextContent.TextWrapping = (MainTextContent.TextWrapping == TextWrapping.Wrap)?
            TextWrapping.NoWrap:TextWrapping.Wrap;
    }
    
    private void RefreshInfo()
    {
        string content = MainTextContent.Text;
        int countOfChars = content.Length;
        string[] lines = content.Split("\n");
        int countOfLines = lines.Length;
        string[] words = content.Split(" ");
        int countOfWords = words.Length;
        CharsCount.Text = $"Символов: {countOfChars}";
        WordsCount.Text = $"Слов: {countOfWords}";
        LinesCount.Text = $"Строк: {countOfLines}";
    }

    private void Button_RefreshInfo_OnClick(object? sender, RoutedEventArgs e)
    {
        RefreshInfo();
    }

    private void MenuItem_ChangeInfoPanelVisibility_OnClick(object? sender, RoutedEventArgs e)
    {
        InfoPanel.IsVisible = (InfoPanel.IsVisible != true);
    }
}