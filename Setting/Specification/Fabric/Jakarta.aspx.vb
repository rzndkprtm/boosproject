Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Fabric_Jakarta
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/fabric", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            txtSearch.Text = Session("SearchFabricJakarta")

            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Session("SearchFabricJakarta") = txtSearch.Text

        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Fabric Group (JKT)"

            BindFabric()

            ddlFabric.Enabled = True

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
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
            Session("SearchFabricJakarta") = txtSearch.Text

            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Blind Type"

                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM FabricGroupLocals WHERE Id='" & lblId.Text & "'")
                    If myData Is Nothing Then Exit Sub

                    BindFabric()

                    ddlFabric.SelectedValue = myData("Id").ToString()
                    txtRoller.Text = myData("Roller").ToString()
                    txtRoman.Text = myData("Roman").ToString()
                    txtPanel.Text = myData("Panel").ToString()

                    ddlFabric.Enabled = False

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Catch ex As Exception
                    MessageError_Process(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If ddlFabric.SelectedValue = "" Then
                MessageError_Process(True, "FABRIC IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                If lblAction.Text = "Add" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO FabricGroupLocals VALUES (@Id, @Panel, @Roller, @Roman)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", ddlFabric.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Panel", txtPanel.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Roller", txtRoller.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Roman", txtRoman.Text.Trim())

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"FabricGroupLocals", ddlFabric.SelectedValue, Session("LoginId").ToString(), "Fabric Group (JKT) Created"}
                    settingClass.Logs(dataLog)

                    Session("SearchFabricJakarta") = txtSearch.Text
                    Response.Redirect("~/setting/specification/fabric/jakarta", False)
                End If

                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE FabricGroupLocals SET Panel=@Panel, Roller=@Roller, Roman=@Roman WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", ddlFabric.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Panel", txtPanel.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Roller", txtRoller.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Roman", txtRoman.Text.Trim())

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"FabricGroupLocals", ddlFabric.SelectedValue, Session("LoginId").ToString(), "Fabric Group (JKT) Updated"}
                    settingClass.Logs(dataLog)

                    Session("SearchFabricJakarta") = txtSearch.Text
                    Response.Redirect("~/setting/specification/fabric/jakarta", False)
                End If
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim dataId As String = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM FabricGroupLocals WHERE Id=@Id UPDATE Logins SET Active=0 WHERE CustomerId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", dataId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Session("SearchFabricJakarta") = txtSearch.Text
            Response.Redirect("~/setting/specification/fabric/jakarta", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchFabricJakarta") = String.Empty
        Try
            Dim stringSearch As String = String.Empty
            If Not searchText = "" Then
                stringSearch = "WHERE Fabrics.Name LIKE '%" & searchText & "%'"
            End If

            Dim thisString As String = String.Format("SELECT FabricGroupLocals.*, Fabrics.Name AS FabricName FROM FabricGroupLocals LEFT JOIN Fabrics ON FabricGroupLocals.Id=Fabrics.Id {0}", stringSearch)
            gvList.DataSource = settingClass.GetDataTable(thisString)
            gvList.DataBind()
            gvList.Columns(1).Visible = LoginAccess("Visible ID")

            btnAdd.Visible = LoginAccess("Add")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindFabric()
        ddlFabric.Items.Clear()
        Try
            ddlFabric.DataSource = settingClass.GetDataTable("SELECT * FROM Fabrics ORDER BY Name ASC")
            ddlFabric.DataTextField = "Name"
            ddlFabric.DataValueField = "Id"
            ddlFabric.DataBind()

            If ddlFabric.Items.Count > 0 Then
                ddlFabric.Items.Insert(0, New ListItem("", ""))
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
