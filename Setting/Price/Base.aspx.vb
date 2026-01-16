Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Partial Class Setting_Price_Base
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Dim settingClass As New SettingClass

    Dim enUS As CultureInfo = New CultureInfo("en-US")
    Dim idIDR As New CultureInfo("id-ID")

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            ddlSortCategory.SelectedValue = Session("PriceBaseCategory")
            BindPriceGroup_Sort()
            ddlSortPriceGroup.SelectedValue = Session("PriceBasePriceGroup")
            txtSearch.Text = Session("PriceBaseSearch")
            BindData(ddlSortCategory.SelectedValue, ddlSortPriceGroup.SelectedValue, txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Price Base"

            BindProductGroup()
            BindPriceGroup()

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub ddlSortCategory_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(ddlSortCategory.SelectedValue, ddlSortPriceGroup.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub ddlSortPriceGroup_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(ddlSortCategory.SelectedValue, ddlSortPriceGroup.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(ddlSortCategory.SelectedValue, ddlSortPriceGroup.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(ddlSortCategory.SelectedValue, ddlSortPriceGroup.SelectedValue, txtSearch.Text)
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
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Price Base"

                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM PriceBases WHERE Id='" & lblId.Text & "'")
                    If myData Is Nothing Then Exit Sub

                    BindProductGroup()
                    BindPriceGroup()

                    ddlCategory.SelectedValue = myData("Category").ToString()
                    ddlMethod.SelectedValue = myData("Method").ToString()
                    ddlProductGroup.SelectedValue = myData("ProductGroupId").ToString()
                    ddlPriceGroup.SelectedValue = myData("PriceGroupId").ToString()
                    txtHeight.Text = myData("Height").ToString()
                    txtWidth.Text = myData("Width").ToString()
                    txtPrice.Text = Convert.ToDecimal(myData("Price")).ToString("G29", enUS)

                    If ddlPriceGroup.SelectedValue = "2" OrElse ddlPriceGroup.SelectedValue = "3" OrElse ddlPriceGroup.SelectedValue = "4" OrElse ddlPriceGroup.SelectedValue = "5" OrElse ddlPriceGroup.SelectedValue = "10" Then
                        txtPrice.Text = Convert.ToDecimal(myData("Price")).ToString("G29", idIDR)
                    End If

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Catch ex As Exception
                    MessageError_Process(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingClass.GetDataTable("SELECT * FROM Logs WHERE DataId='" & dataId & "' AND Type='PriceBases' ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Log(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If ddlCategory.SelectedValue = "" Then
                MessageError_Process(True, "CATEGORY IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If ddlMethod.SelectedValue = "" Then
                MessageError_Process(True, "METHOD IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If ddlProductGroup.SelectedValue = "" Then
                MessageError_Process(True, "PRODUCT GROUP IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If ddlPriceGroup.SelectedValue = "" Then
                MessageError_Process(True, "PRICE GROUP IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtHeight.Text = "" Then
                MessageError_Process(True, "HEIGHT IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtWidth.Text = "" Then
                MessageError_Process(True, "WIDTH IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                If lblAction.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM PriceBases ORDER BY Id DESC")

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO PriceBases VALUES (@Id, @Category, @Method, @ProductGroupId, @PriceGroupId, @Height, @Width, @Price)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@Category", ddlCategory.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Method", ddlMethod.SelectedValue)
                            myCmd.Parameters.AddWithValue("@ProductGroupId", ddlProductGroup.SelectedValue)
                            myCmd.Parameters.AddWithValue("@PriceGroupId", ddlPriceGroup.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Height", txtHeight.Text)
                            myCmd.Parameters.AddWithValue("@Width", txtWidth.Text)
                            myCmd.Parameters.AddWithValue("@Price", txtPrice.Text)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"PriceBases", thisId, Session("LoginId").ToString(), "Price Base Created"}
                    settingClass.Logs(dataLog)

                    Session("PriceBaseCategory") = ddlSortCategory.SelectedValue
                    Session("PriceBasePriceGroup") = ddlSortPriceGroup.SelectedValue
                    Session("PriceBaseSearch") = txtSearch.Text

                    Response.Redirect("~/setting/price/base", False)
                End If

                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE PriceBases SET Category=@Category, Method=@Method, ProductGroupId=@ProductGroupId, PriceGroupId=@PriceGroupId, Height=@Height, Width=@Width, Price=@Price WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@Category", ddlCategory.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Method", ddlMethod.SelectedValue)
                            myCmd.Parameters.AddWithValue("@ProductGroupId", ddlProductGroup.SelectedValue)
                            myCmd.Parameters.AddWithValue("@PriceGroupId", ddlPriceGroup.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Height", txtHeight.Text)
                            myCmd.Parameters.AddWithValue("@Width", txtWidth.Text)
                            myCmd.Parameters.AddWithValue("@Price", txtPrice.Text)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"PriceBases", lblId.Text, Session("LoginId").ToString(), "Price Base Updated"}
                    settingClass.Logs(dataLog)

                    Session("PriceBaseCategory") = ddlSortCategory.SelectedValue
                    Session("PriceBasePriceGroup") = ddlSortPriceGroup.SelectedValue
                    Session("PriceBaseSearch") = txtSearch.Text

                    Response.Redirect("~/setting/price/base", False)
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

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM PriceBases WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='PriceBases' AND DataId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            Session("PriceBaseCategory") = ddlSortCategory.SelectedValue
            Session("PriceBasePriceGroup") = ddlSortPriceGroup.SelectedValue
            Session("PriceBaseSearch") = txtSearch.Text

            Response.Redirect("~/setting/price/base", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(category As String, priceGroup As String, search As String)
        Session("PriceBaseCategory") = String.Empty
        Session("PriceBasePriceGroup") = String.Empty
        Session("PriceBaseSearch") = String.Empty
        Try
            Dim conditions As New List(Of String)

            If Not String.IsNullOrEmpty(category) Then
                conditions.Add("PriceBases.Category = '" & category & "'")
            End If

            If Not String.IsNullOrEmpty(priceGroup) Then
                conditions.Add("PriceBases.PriceGroupId = '" & priceGroup & "'")
            End If

            If Not String.IsNullOrEmpty(search) Then
                conditions.Add("PriceProductGroups.Name LIKE '%" & search & "%'")
            End If

            Dim whereClause As String = String.Empty
            If conditions.Count > 0 Then
                whereClause = "WHERE " & String.Join(" AND ", conditions)
            End If

            Dim sql As String = String.Format("SELECT PriceBases.*, PriceProductGroups.Name AS ProductGroupName, PriceGroups.Name AS PriceGroupName FROM PriceBases LEFT JOIN PriceProductGroups ON PriceBases.ProductGroupId = PriceProductGroups.Id LEFT JOIN PriceGroups ON PriceBases.PriceGroupId = PriceGroups.Id {0} ORDER BY PriceProductGroups.Name, PriceGroups.Name, PriceBases.Height, PriceBases.Width ASC", whereClause)

            gvList.DataSource = settingClass.GetDataTable(sql)
            gvList.DataBind()
            gvList.Columns(1).Visible = PageAction("Visible ID")
            gvList.Columns(2).Visible = PageAction("Visible Category")

            btnAdd.Visible = PageAction("Add")
            btnImport.Visible = PageAction("Import")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindProductGroup()
        ddlProductGroup.Items.Clear()
        Try
            ddlProductGroup.DataSource = settingClass.GetDataTable("SELECT * FROM PriceProductGroups ORDER BY Id ASC")
            ddlProductGroup.DataTextField = "Name"
            ddlProductGroup.DataValueField = "Id"
            ddlProductGroup.DataBind()

            If ddlProductGroup.Items.Count > 0 Then
                ddlProductGroup.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlProductGroup.Items.Clear()
        End Try
    End Sub

    Protected Sub BindPriceGroup()
        ddlPriceGroup.Items.Clear()
        Try
            ddlPriceGroup.DataSource = settingClass.GetDataTable("SELECT * FROM PriceGroups ORDER BY Id ASC")
            ddlPriceGroup.DataTextField = "Name"
            ddlPriceGroup.DataValueField = "Id"
            ddlPriceGroup.DataBind()

            If ddlPriceGroup.Items.Count > 0 Then
                ddlPriceGroup.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlPriceGroup.Items.Clear()
        End Try
    End Sub

    Protected Sub BindPriceGroup_Sort()
        ddlSortPriceGroup.Items.Clear()
        Try
            ddlSortPriceGroup.DataSource = settingClass.GetDataTable("SELECT * FROM PriceGroups ORDER BY Id ASC")
            ddlSortPriceGroup.DataTextField = "Name"
            ddlSortPriceGroup.DataValueField = "Id"
            ddlSortPriceGroup.DataBind()

            If ddlSortPriceGroup.Items.Count > 0 Then
                ddlSortPriceGroup.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlSortPriceGroup.Items.Clear()
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Process(visible As Boolean, message As String)
        divErrorProcess.Visible = visible : msgErrorProcess.InnerText = message
    End Sub

    Protected Sub MessageError_Log(visible As Boolean, message As String)
        divErrorLog.Visible = visible : msgErrorLog.InnerText = message
    End Sub

    Protected Function BindTextLog(logId As String) As String
        Return settingClass.getTextLog(logId)
    End Function

    Protected Function BindCost(cost As Decimal, priceGroupId As String) As String
        If cost > 0 Then
            If priceGroupId = "2" OrElse priceGroupId = "3" OrElse priceGroupId = "4" OrElse priceGroupId = "5" Then
                Return cost.ToString("N2", idIDR)
            End If
            Return cost.ToString("N2", enUS)
        End If

        Return String.Empty
    End Function

    Protected Function PageAction(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim actionClass As New ActionClass

            Return actionClass.GetActionAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
