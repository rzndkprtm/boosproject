Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Job_Sheet_Detail_Default
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/job/sheet", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("sheetid")) Then
            Response.Redirect("~/setting/job/sheet", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("sheetid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
            BindDataDetail(lblId.Text)
        End If
    End Sub

    Protected Sub btnEdit_Click(sender As Object, e As EventArgs)
        url = String.Format("~/setting/job/sheet/edit?sheetid={0}&returnpage=detail", lblId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnAddDetail_Click(sender As Object, e As EventArgs)
        url = String.Format("~/setting/job/sheet/detail/add?sheetid={0}", lblId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnSortOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            For Each row As GridViewRow In gvListSortOrder.Rows
                If row.RowType = DataControlRowType.DataRow Then
                    Dim thisId As String = gvListSortOrder.DataKeys(row.RowIndex).Values("Id").ToString()

                    Dim txtSortOrder As TextBox = CType(row.FindControl("txtSortOrder"), TextBox)
                    Dim sortOrder As String = txtSortOrder.Text

                    Using thisConn As New SqlConnection(myConn)
                        Using thisCmd As New SqlCommand("UPDATE JobSheetDetails SET SortOrder=@SortOrder WHERE Id=@Id", thisConn)
                            thisCmd.Parameters.AddWithValue("@Id", thisId)
                            thisCmd.Parameters.AddWithValue("@SortOrder", sortOrder)
                            thisConn.Open()
                            thisCmd.ExecuteNonQuery()
                        End Using
                    End Using
                End If
            Next

            url = String.Format("~/setting/job/sheet/detail/?sheetid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnDeleteDetail_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub BindData(sheetId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT *, CASE WHEN Active=1 THEN 'Yes' WHEN Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM JobSheets WHERE Id='" & sheetId & "'")
            If thisData Is Nothing Then
                Response.Redirect("~/setting/job/sheet", False)
                Exit Sub
            End If

            lblName.Text = thisData("Name").ToString()
            lblAlias.Text = thisData("Alias").ToString()
            lblDescription.Text = thisData("Description").ToString()
            lblActive.Text = thisData("DataActive").ToString()

            btnEdit.Visible = LoginAccess("Edit")
        Catch ex As Exception
            MessageError(True, ex.ToString)
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDataDetail(sheetId As String)
        Try
            Dim thisString As String = "SELECT *, CASE WHEN Active=1 THEN 'Yes' WHEN Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM JobSheetDetails WHERE JobSheetId='" & sheetId & "' ORDER BY SortOrder ASC"
            gvListDetail.DataSource = settingClass.GetDataTable(thisString)
            gvListDetail.DataBind()

            gvListSortOrder.DataSource = settingClass.GetDataTable(thisString)
            gvListSortOrder.DataBind()

            btnAddDetail.Visible = LoginAccess("Add Detail")
            aSortOrder.Visible = LoginAccess("Sort Detail")
        Catch ex As Exception
            MessageError(True, ex.ToString)
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

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
