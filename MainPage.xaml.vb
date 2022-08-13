Imports Windows.ApplicationModel.DataTransfer
Imports Windows.Storage
Imports Windows.UI
Imports Windows.UI.ViewManagement.Core
Imports Windows.UI.Xaml.Shapes

Partial Public NotInheritable Class MainPage
    Inherits Page
    ''Variables
    Private InputTextName As String = ""
    Private ReadOnly ListData As New List(Of String)()
    Private ReadOnly RowNumber As Integer
    Private ReadOnly Timer As DispatcherTimer
    Public Sub New()
        InitializeComponent()
        Timer = New DispatcherTimer() With {.Interval = TimeSpan.FromSeconds(5)}
        AddHandler Timer.Tick, AddressOf Timer_Tick
        Timer.Start()
    End Sub
    ''CreateListItem subroutine
    Private Sub CreateListItem(InputText As String)
        If Not InputText = "" Then
            Dim AddInputText As New TextBlock With {.Foreground = New SolidColorBrush(Colors.Gray), .Name = InputTextName, .Text = InputText}
            AddInputText.SetValue(Grid.RowProperty, RowNumber)
            AddInputText.SetValue(Grid.ColumnProperty, 1)
            AddInputText.Margin = New Thickness(15, 2, 10, 0)
            AddInputText.VerticalAlignment = 1
            ListData.Add(InputText)
        End If
    End Sub
    ''Navi subroutines
    Private Sub Navi_ContainsFullScreenElementChanged(sender As WebView, args As Object) Handles Navi.ContainsFullScreenElementChanged
        Dim NaviApplicationView As ApplicationView = ApplicationView.GetForCurrentView()
        If sender.ContainsFullScreenElement Then
            NaviApplicationView.TryEnterFullScreenMode()
        Else
            NaviApplicationView.ExitFullScreenMode()
        End If
    End Sub
    Private Sub Navi_NavigationCompleted(sender As Object, e As WebViewNavigationCompletedEventArgs) Handles Navi.NavigationCompleted
        WebsiteTitleBlock.Text = Navi.DocumentTitle.ToString()
        SearchBox.Text = Navi.Source.ToString()
        Dim GetInputText As TextBox = TryCast(FindName("SearchBox"), TextBox)
        Dim ObjectTextBox As String = GetInputText.Text
        InputTextName = "ListTextBox_"
        CreateListItem(ObjectTextBox)
        RefreshButton.Visibility = 0
        StopButton.Visibility = 1
        If Navi.CanGoForward = True Then
            ForwardButton.IsEnabled = True
        Else
            ForwardButton.IsEnabled = False
        End If
        If Navi.CanGoBack = True Then
            BackButton.IsEnabled = True
        Else
            BackButton.IsEnabled = False
        End If
        EmojiButton.IsEnabled = True
        HandwritingButton.IsEnabled = True
        HomeButton.IsEnabled = True
        IndexedDBSwitch.IsEnabled = True
        JavaScriptSwitch.IsEnabled = True
        KeyboardButton.IsEnabled = True
        RefreshButton.IsEnabled = True
        SearchBingButton.IsEnabled = True
        SearchBox.IsEnabled = True
        ShareContentButton.IsEnabled = True
        ShareLinkButton.IsEnabled = True
        SpellCheckSwitch.IsEnabled = True
    End Sub
    Private Sub Navi_NavigationStarting(sender As Object, e As WebViewNavigationStartingEventArgs) Handles Navi.NavigationStarting
        RefreshButton.Visibility = 1
        StopButton.Visibility = 0
    End Sub
    ''Navigation subroutines
    Protected Overrides Async Sub OnNavigatedFrom(e As NavigationEventArgs)
        MyBase.OnNavigatedTo(e)
        Dim CurrentInputPane As InputPane = InputPane.GetForCurrentView()
        AddHandler CurrentInputPane.Hiding, AddressOf OnHiding
        AddHandler CurrentInputPane.Showing, AddressOf OnShowing
        Try
            Dim OwanderStorageFolder As StorageFolder = ApplicationData.Current.LocalFolder
            Dim OwanderListFile As StorageFile = Await OwanderStorageFolder.CreateFileAsync("OwanderAddress.txt", 1)
            Await FileIO.WriteTextAsync(OwanderListFile, Navi.Source.ToString())
        Catch __unusedException1__ As Exception
            Debug.WriteLine("Sorry, the file was not found")
        End Try
    End Sub
    Protected Overrides Async Sub OnNavigatedTo(e As NavigationEventArgs)
        MyBase.OnNavigatedFrom(e)
        Dim CurrentInputPane As InputPane = InputPane.GetForCurrentView()
        AddHandler CurrentInputPane.Hiding, AddressOf OnHiding
        AddHandler CurrentInputPane.Showing, AddressOf OnShowing
        Try
            Dim OwanderStorageFolder As StorageFolder = ApplicationData.Current.LocalFolder
            Dim OwanderFile = Await OwanderStorageFolder.GetFileAsync("OwanderAddress.txt")
            Dim ReadOwanderFile = Await FileIO.ReadLinesAsync(OwanderFile)
            Dim NumberLines As Integer = 0
            ListData.Clear()
            For Each line In ReadOwanderFile
                Navi.Navigate(New Uri(line))
            Next
        Catch __unusedException1__ As Exception
            Debug.WriteLine("Sorry, the file was not found")
        End Try
        IndexedDBSwitch.IsOn = True
        JavaScriptSwitch.IsOn = True
        SpellCheckSwitch.IsOn = True
    End Sub
    ''OnHiding and OnShowing subroutines
    Private Sub OnHiding(sender As InputPane, e As InputPaneVisibilityEventArgs)
        KeyboardButton.Label = "Keyboard"
    End Sub
    Private Sub OnShowing(sender As InputPane, e As InputPaneVisibilityEventArgs)
        KeyboardButton.Label = "Showing keyboard"
    End Sub
    ''SearchBingButton subroutine
    Private Sub SearchBingButton_Click(sender As Object, e As RoutedEventArgs) Handles SearchBingButton.Click
        Navi.Navigate(New Uri("https://www.bing.com/search?q=" & SearchBox.Text))
    End Sub
    ''SearchBox subroutine
    Private Async Sub SearchBox_KeyDown(sender As Object, e As KeyRoutedEventArgs) Handles SearchBox.KeyDown
        If e.Key = 13 Then
            If String.IsNullOrEmpty(SearchBox.Text) Then
                Dim CloseCornerRadius As CornerRadius
                CloseCornerRadius.BottomLeft = 2
                CloseCornerRadius.BottomRight = 2
                CloseCornerRadius.TopLeft = 2
                CloseCornerRadius.TopRight = 2
                Dim DialogBorderThickness As Thickness
                DialogBorderThickness.Bottom = 0
                DialogBorderThickness.Left = 0
                DialogBorderThickness.Right = 0
                DialogBorderThickness.Top = 0
                Dim DialogCornerRadius As CornerRadius
                DialogCornerRadius.BottomLeft = 4
                DialogCornerRadius.BottomRight = 4
                DialogCornerRadius.TopLeft = 4
                DialogCornerRadius.TopRight = 4
                Dim CloseButtonStyle As New Style(GetType(Button))
                CloseButtonStyle.Setters.Add(New Setter(BorderThicknessProperty, DialogBorderThickness))
                CloseButtonStyle.Setters.Add(New Setter(CornerRadiusProperty, CloseCornerRadius))
                Dim EmptySearchBoxDialog As New ContentDialog() With {.BorderThickness = DialogBorderThickness, .CloseButtonStyle = CloseButtonStyle, .CloseButtonText = "Close", .Content = "Please enter a valid URL into the search box.", .CornerRadius = DialogCornerRadius, .Title = "Search Box Empty"}
                Await EmptySearchBoxDialog.ShowAsync()
            Else
                Navi.Navigate(New Uri("https://" & SearchBox.Text))
            End If
        End If
    End Sub
    ''ShareContentDataRequested and ShareContentSourceLoad subroutines
    Private Sub ShareContentDataRequested(sender As DataTransferManager, e As DataRequestedEventArgs)
        Dim Request As DataRequest = e.Request
        Dim Deferral As DataRequestDeferral = Request.GetDeferral()
        Try
            Dim requestDataOperation = Navi.CaptureSelectedContentToDataPackageAsync()
            requestDataOperation.Completed = Sub(asyncInfo, status)
                                                 Dim RequestData As DataPackage = asyncInfo.GetResults()
                                                 If (RequestData IsNot Nothing) AndAlso (RequestData.GetView().AvailableFormats.Count > 0) Then
                                                     RequestData.Properties.ApplicationName = "Owander"
                                                     RequestData.Properties.Description = "This is some web content from Owander."
                                                     RequestData.Properties.ContentSourceWebLink = New Uri(Navi.Source.ToString())
                                                     RequestData.Properties.Title = "Shared Owander Content"
                                                     Request.Data = RequestData
                                                     Deferral.Complete()
                                                 Else
                                                     Request.FailWithDisplayText("Please select content from inside the browser.")
                                                 End If
                                             End Sub
        Catch __unusedException1__ As Exception
            Deferral.Complete()
        End Try
    End Sub
    Private Sub ShareContentSourceLoad()
        Dim OwanderDataTransferManager As DataTransferManager = DataTransferManager.GetForCurrentView()
        AddHandler OwanderDataTransferManager.DataRequested, New TypedEventHandler(Of DataTransferManager, DataRequestedEventArgs)(AddressOf ShareContentDataRequested)
        DataTransferManager.ShowShareUI()
    End Sub
    ''ShareLinkDataRequested and ShareLinkSourceLoad subroutines
    Private Sub ShareLinkDataRequested(sender As DataTransferManager, e As DataRequestedEventArgs)
        Dim Request As DataRequest = e.Request
        Request.Data.SetWebLink(Navi.Source)
        Request.Data.Properties.ApplicationName = "Owander"
        Request.Data.Properties.Description = "This is a shared link from Owander."
        Request.Data.Properties.Title = "Shared Owander Link"
    End Sub
    Private Sub ShareLinkSourceLoad()
        Dim OwanderDataTransferManager As DataTransferManager = DataTransferManager.GetForCurrentView()
        AddHandler OwanderDataTransferManager.DataRequested, New TypedEventHandler(Of DataTransferManager, DataRequestedEventArgs)(AddressOf ShareLinkDataRequested)
        DataTransferManager.ShowShareUI()
    End Sub
    ''SplitViewMenu subroutines
    Private Sub IndexedDBSwitch_Toggled(sender As Object, e As RoutedEventArgs) Handles IndexedDBSwitch.Toggled
        If IndexedDBSwitch.IsOn = True Then
            Navi.Settings.IsIndexedDBEnabled = True
        End If
        If IndexedDBSwitch.IsOn = False Then
            Navi.Settings.IsIndexedDBEnabled = False
        End If
    End Sub
    Private Sub JavaScriptSwitch_Toggled(sender As Object, e As RoutedEventArgs) Handles JavaScriptSwitch.Toggled
        If JavaScriptSwitch.IsOn = True Then
            Navi.Settings.IsJavaScriptEnabled = True
        End If
        If JavaScriptSwitch.IsOn = False Then
            Navi.Settings.IsJavaScriptEnabled = False
        End If
    End Sub
    Private Sub MenuButton_Click(sender As Object, e As RoutedEventArgs) Handles MenuButton.Click
        Dim Back As New SymbolIcon With {.Symbol = 57618}
        Dim Open As New SymbolIcon With {.Symbol = 59136}
        SplitViewMenu.IsPaneOpen = Not SplitViewMenu.IsPaneOpen
        If SplitViewMenu.IsPaneOpen = True Then
            MenuButton.Content = Back
        End If
        If SplitViewMenu.IsPaneOpen = False Then
            MenuButton.Content = Open
        End If
    End Sub
    Private Sub SettingsButton_Click(sender As Object, e As RoutedEventArgs) Handles SettingsButton.Click
        Dim Back As New SymbolIcon With {.Symbol = 57618}
        Dim Open As New SymbolIcon With {.Symbol = 59136}
        SplitViewMenu.IsPaneOpen = Not SplitViewMenu.IsPaneOpen
        If SplitViewMenu.IsPaneOpen = True Then
            MenuButton.Content = Back
            IndexedDBSwitch.Visibility = 0
            JavaScriptSwitch.Visibility = 0
            SpellCheckSwitch.Visibility = 0
        End If
        If SplitViewMenu.IsPaneOpen = False Then
            MenuButton.Content = Open
            IndexedDBSwitch.Visibility = 1
            JavaScriptSwitch.Visibility = 1
            SpellCheckSwitch.Visibility = 1
        End If
    End Sub
    Private Sub SpellCheckSwitch_Toggled(sender As Object, e As RoutedEventArgs) Handles SpellCheckSwitch.Toggled
        If SpellCheckSwitch.IsOn = True Then
            SearchBox.IsSpellCheckEnabled = True
        End If
        If SpellCheckSwitch.IsOn = False Then
            SearchBox.IsSpellCheckEnabled = False
        End If
    End Sub
    ''Timer subroutine
    Private Async Sub Timer_Tick(sender As Object, e As Object)
        Try
            Dim OwanderStorageFolder As StorageFolder = ApplicationData.Current.LocalFolder
            Dim OwanderListFile As StorageFile = Await OwanderStorageFolder.CreateFileAsync("OwanderAddress.txt", 1)
            Await FileIO.WriteTextAsync(OwanderListFile, Navi.Source.ToString())
        Catch __unusedException1__ As Exception
            Debug.WriteLine("Sorry, the file was not found")
        End Try
    End Sub
    ''OwanderBar subroutines
    Private Sub BackButton_Click(sender As Object, e As RoutedEventArgs) Handles BackButton.Click
        If Navi.CanGoBack Then
            Navi.GoBack()
        End If
    End Sub
    Private Sub EmojiButton_Click(sender As Object, e As RoutedEventArgs) Handles EmojiButton.Click
        CoreInputView.GetForCurrentView().TryShow(3)
    End Sub
    Private Sub ForwardButton_Click(sender As Object, e As RoutedEventArgs) Handles ForwardButton.Click
        If Navi.CanGoForward Then
            Navi.GoForward()
        End If
    End Sub
    Private Sub HandwritingButton_Click(sender As Object, e As RoutedEventArgs) Handles HandwritingButton.Click
        CoreInputView.GetForCurrentView().TryShow(2)
    End Sub
    Private Sub HomeButton_Click(sender As Object, e As RoutedEventArgs) Handles HomeButton.Click
        Navi.Navigate(New Uri("ms-appx-web:///file/welcome.html"))
    End Sub
    Private Sub KeyboardButton_Click(sender As Object, e As RoutedEventArgs) Handles KeyboardButton.Click
        CoreInputView.GetForCurrentView().TryShow(1)
    End Sub
    Private Sub RefreshButton_Click(sender As Object, e As RoutedEventArgs) Handles RefreshButton.Click
        Navi.Refresh()
        RefreshButton.Visibility = 1
        StopButton.Visibility = 0
    End Sub
    Private Sub ShareContentButton_Click(sender As Object, e As RoutedEventArgs) Handles ShareContentButton.Click
        ShareContentSourceLoad()
    End Sub
    Private Sub ShareLinkButton_Click(sender As Object, e As RoutedEventArgs) Handles ShareLinkButton.Click
        ShareLinkSourceLoad()
    End Sub
    Private Sub StopButton_Click(sender As Object, e As RoutedEventArgs) Handles StopButton.Click
        Navi.Refresh()
        RefreshButton.Visibility = 0
        StopButton.Visibility = 1
    End Sub
End Class