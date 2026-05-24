Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Product_Alias
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/product", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchProductAlias")
            BindData(txtSearch.Text)
            BindProduct()
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Session("SearchProductAlias") = txtSearch.Text
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Product Alias"

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()

            Session("SearchProductAlias") = txtSearch.Text
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Alias"

                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM ProductAlias WHERE Id='" & lblId.Text & "'")
                    If myData Is Nothing Then Exit Sub

                    ddlFirstId.SelectedValue = myData("FirstID").ToString()
                    ddlSecondId.SelectedValue = myData("SecondID").ToString()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Catch ex As Exception
                    MessageError_Process(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If ddlFirstId.SelectedValue = "" Then
                MessageError_Process(True, "FIRST PRODUCT IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If ddlSecondId.SelectedValue = "" Then
                MessageError_Process(True, "SECOND PRODUCT IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                If lblAction.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM ProductAlias ORDER BY Id DESC")

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO ProductAlias VALUES (@Id, @FirstId, @SecondId)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@Type", lblType.Text)
                            myCmd.Parameters.AddWithValue("@FirstId", ddlFirstId.SelectedValue)
                            myCmd.Parameters.AddWithValue("@SecondId", ddlSecondId.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"ProductAlias", thisId, Session("LoginId").ToString(), "Alias Created"}
                    settingClass.Logs(dataLog)

                    Session("SearchProductAlias") = txtSearch.Text
                    Response.Redirect("~/setting/specification/product/alias", False)
                End If

                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE ProductAlias SET FirstId=@FirstId, SecondId=@SecondId WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@FirstId", ddlFirstId.SelectedValue)
                            myCmd.Parameters.AddWithValue("@SecondId", ddlSecondId.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"ProductAlias", lblId.Text, Session("LoginId").ToString(), "Alias Updated"}
                    settingClass.Logs(dataLog)

                    Session("SearchProductAlias") = txtSearch.Text
                    Response.Redirect("~/setting/specification/product/alias", False)
                End If
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM ProductAlias WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='ProductAlias' AND DataId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            Session("SearchProductAlias") = txtSearch.Text
            Response.Redirect("~/setting/specification/product/alias", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchProductAlias") = String.Empty
        Try
            Dim stringSearch As String = String.Empty
            If Not searchText = "" Then
                stringSearch = "WHERE PI.Name LIKE '%" & searchText & "%' OR P2.Name LIKE '%" & searchText & "%'"
            End If

            Dim thisString As String = String.Format("SELECT ProductAlias.*, P1.Name AS FirstName, P2.Name AS SecondName FROM ProductAlias LEFT JOIN Products P1 ON ProductAlias.FirstId = P1.Id LEFT JOIN Products P2 ON ProductAlias.SecondId = P2.Id {0} ORDER BY ProductAlias.Id ASC", stringSearch)
            gvList.DataSource = settingClass.GetDataTable(thisString)
            gvList.DataBind()

            btnAdd.Visible = LoginAccess("Add")
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindProduct()
        ddlFirstId.Items.Clear() : ddlSecondId.Items.Clear()
        Try
            ddlFirstId.DataSource = settingClass.GetDataTable("SELECT * FROM Products ORDER BY Name ASC")
            ddlFirstId.DataTextField = "Name"
            ddlFirstId.DataValueField = "Id"
            ddlFirstId.DataBind()

            ddlSecondId.DataSource = settingClass.GetDataTable("SELECT * FROM Products ORDER BY Name ASC")
            ddlSecondId.DataTextField = "Name"
            ddlSecondId.DataValueField = "Id"
            ddlSecondId.DataBind()

            If ddlFirstId.Items.Count > 0 Then
                ddlFirstId.Items.Insert(0, New ListItem("", ""))
            End If
            If ddlSecondId.Items.Count > 0 Then
                ddlSecondId.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Process(visible As Boolean, message As String)
        divErrorProcess.Visible = visible : msgErrorProcess.InnerText = message
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
