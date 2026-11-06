Imports System.Data
Imports System.Data.SqlClient

Partial Class Guide
    Inherits Page

    Dim settingClass As New SettingClass

    Private ReadOnly ConnectionString As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindTutorial(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindTutorial(txtSearch.Text)
    End Sub

    Private Sub BindTutorial(searchText As String)
        MessageError(False, String.Empty)
        Try
            pnlEmpty.Visible = False

            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", If(String.IsNullOrWhiteSpace(searchText), CType(DBNull.Value, Object), searchText)),
                New SqlParameter("@CompanyId", If(Session("CompanyId") Is Nothing OrElse String.IsNullOrWhiteSpace(Session("CompanyId").ToString()), CType(DBNull.Value, Object), Convert.ToInt32(Session("CompanyId")))),
                New SqlParameter("@RoleName", Session("RoleName").ToString())
            }

            Dim dt As DataTable = settingClass.GetDataTableSP("sp_Tutorials_List", params)

            rptTutorial.DataSource = dt
            rptTutorial.DataBind()

            pnlEmpty.Visible = (dt.Rows.Count = 0)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        pnlError.Visible = visible : lblError.Text = message
    End Sub

    Protected Function GetFileUrl(value As Object) As String
        Try
            If value Is Nothing OrElse value Is DBNull.Value Then Return String.Empty

            Dim filePath As String = value.ToString().Trim()

            If String.IsNullOrWhiteSpace(filePath) Then Return String.Empty

            filePath = filePath.Replace("\", "/")

            If filePath.StartsWith("~/File/Tutorial/", StringComparison.OrdinalIgnoreCase) Then
                Return ResolveUrl(filePath)
            End If

            Return String.Empty
        Catch
            Return String.Empty
        End Try
    End Function

    Protected Function IsValidUrl(value As Object) As Boolean
        Try
            If value Is Nothing OrElse value Is DBNull.Value Then Return False

            Dim url As String = value.ToString().Trim()

            If String.IsNullOrWhiteSpace(url) Then Return False

            Dim uri As Uri = Nothing

            If Uri.TryCreate(url, UriKind.Absolute, uri) Then
                Return uri.Scheme = Uri.UriSchemeHttp OrElse uri.Scheme = Uri.UriSchemeHttps
            End If

            Return False
        Catch
            Return False
        End Try
    End Function

    Protected Function GetSafeUrl(value As Object) As String
        Try
            If IsValidUrl(value) Then Return value.ToString().Trim()
            Return "#"
        Catch
            Return "#"
        End Try
    End Function

    Protected Function LoginAccess(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim accessClass As New AccessClass

            Return accessClass.GetLoginAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
