Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Partial Class Setting_Price_Base_Editable
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim enUS As CultureInfo = New CultureInfo("en-US")
    Dim idIDR As New CultureInfo("id-ID")
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price/base", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            BindProductGroup()
            BindPriceGroup()

            btnAdd.Visible = PageAction("Add")
            btnImport.Visible = PageAction("Import")

            gvList.DataSource = Nothing
            gvList.DataBind()
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Price Base"

            divCategoryMethod.Visible = True
            divProductGroup.Visible = True
            divPriceGroup.Visible = True
            txtHeightProcess.Enabled = True
            txtWidthProcess.Enabled = True

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnImport_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/price/base/import", False)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        gvList.DataSource = Nothing
        gvList.DataBind()
        Try
            If ddlCategory.SelectedValue = "" Then
                gvList.DataSource = Nothing
                gvList.DataBind()
                Exit Sub
            End If
            If ddlMethod.SelectedValue = "" Then
                gvList.DataSource = Nothing
                gvList.DataBind()
                Exit Sub
            End If
            If ddlProductGroup.SelectedValue = "" Then
                gvList.DataSource = Nothing
                gvList.DataBind()
                Exit Sub
            End If
            If ddlPriceGroup.SelectedValue = "" Then
                gvList.DataSource = Nothing
                gvList.DataBind()
                Exit Sub
            End If

            If txtHeight.Text = "" Then txtHeight.Text = "0"
            If txtWidth.Text = "" Then txtWidth.Text = "0"

            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@Category", If(String.IsNullOrEmpty(ddlCategory.SelectedValue), CType(DBNull.Value, Object), ddlCategory.SelectedValue)),
                New SqlParameter("@Method", If(String.IsNullOrEmpty(ddlMethod.SelectedValue), CType(DBNull.Value, Object), ddlMethod.SelectedValue)),
                New SqlParameter("@ProductGroupId", If(String.IsNullOrEmpty(ddlProductGroup.SelectedValue), CType(DBNull.Value, Object), ddlProductGroup.SelectedValue)),
                New SqlParameter("@PriceGroupId", If(String.IsNullOrEmpty(ddlPriceGroup.SelectedValue), CType(DBNull.Value, Object), ddlPriceGroup.SelectedValue)),
                New SqlParameter("@Height", If(String.IsNullOrEmpty(txtHeight.Text), CType(DBNull.Value, Object), txtHeight.Text)),
                New SqlParameter("@Width", If(String.IsNullOrEmpty(txtWidth.Text), CType(DBNull.Value, Object), txtWidth.Text))
            }

            Dim thisData As DataTable = settingClass.GetDataTableSP("sp_PriceBaseList", params)

            gvList.DataSource = thisData
            gvList.DataBind()
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

                    ddlCategoryProcess.SelectedValue = myData("Category").ToString()
                    ddlMethodProcess.SelectedValue = myData("Method").ToString()
                    ddlProductGroupProcess.SelectedValue = myData("ProductGroupId").ToString()
                    ddlPriceGroupProcess.SelectedValue = myData("PriceGroupId").ToString()
                    txtHeightProcess.Text = myData("Height").ToString()
                    txtWidthProcess.Text = myData("Width").ToString()
                    txtPrice.Text = Convert.ToDecimal(myData("Price")).ToString("G29", enUS)
                    txtConditional.Text = myData("Conditional").ToString()

                    If ddlPriceGroupProcess.SelectedValue = "2" OrElse ddlPriceGroupProcess.SelectedValue = "3" OrElse ddlPriceGroupProcess.SelectedValue = "4" OrElse ddlPriceGroup.SelectedValue = "5" OrElse ddlPriceGroupProcess.SelectedValue = "10" Then
                        txtPrice.Text = Convert.ToDecimal(myData("Price")).ToString("G29", idIDR)
                    End If

                    divCategoryMethod.Visible = False
                    divProductGroup.Visible = False
                    divPriceGroup.Visible = False
                    txtHeightProcess.Enabled = False
                    txtWidthProcess.Enabled = False

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
            If ddlCategoryProcess.SelectedValue = "" Then
                MessageError_Process(True, "CATEGORY IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If ddlMethodProcess.SelectedValue = "" Then
                MessageError_Process(True, "METHOD IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If ddlProductGroupProcess.SelectedValue = "" Then
                MessageError_Process(True, "PRODUCT GROUP IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If ddlPriceGroupProcess.SelectedValue = "" Then
                MessageError_Process(True, "PRICE GROUP IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtHeightProcess.Text = "" Then
                MessageError_Process(True, "HEIGHT IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtWidthProcess.Text = "" Then
                MessageError_Process(True, "WIDTH IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                If lblAction.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM PriceBases ORDER BY Id DESC")

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO PriceBases VALUES (@Id, @Category, @Method, @ProductGroupId, @PriceGroupId, @Height, @Width, @Price, @Conditional)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@Category", ddlCategoryProcess.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Method", ddlMethodProcess.SelectedValue)
                            myCmd.Parameters.AddWithValue("@ProductGroupId", ddlProductGroupProcess.SelectedValue)
                            myCmd.Parameters.AddWithValue("@PriceGroupId", ddlPriceGroupProcess.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Height", txtHeightProcess.Text)
                            myCmd.Parameters.AddWithValue("@Width", txtWidthProcess.Text)
                            myCmd.Parameters.AddWithValue("@Price", txtPrice.Text)
                            myCmd.Parameters.AddWithValue("@Conditional", txtConditional.Text)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"PriceBases", thisId, Session("LoginId").ToString(), "Price Base Created"}
                    settingClass.Logs(dataLog)

                    Response.Redirect("~/setting/price/base/editable", False)
                End If

                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE PriceBases SET Category=@Category, Method=@Method, ProductGroupId=@ProductGroupId, PriceGroupId=@PriceGroupId, Height=@Height, Width=@Width, Price=@Price, Conditional=@Conditional WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@Category", ddlCategoryProcess.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Method", ddlMethodProcess.SelectedValue)
                            myCmd.Parameters.AddWithValue("@ProductGroupId", ddlProductGroupProcess.SelectedValue)
                            myCmd.Parameters.AddWithValue("@PriceGroupId", ddlPriceGroupProcess.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Height", txtHeightProcess.Text)
                            myCmd.Parameters.AddWithValue("@Width", txtWidthProcess.Text)
                            myCmd.Parameters.AddWithValue("@Price", txtPrice.Text)
                            myCmd.Parameters.AddWithValue("@Conditional", txtConditional.Text)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"PriceBases", lblId.Text, Session("LoginId").ToString(), "Price Base Updated"}
                    settingClass.Logs(dataLog)

                    Response.Redirect("~/setting/price/base/editable", False)
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

            Response.Redirect("~/setting/price/base/editable", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindProductGroup()
        ddlProductGroup.Items.Clear()
        ddlProductGroupProcess.Items.Clear()
        Try
            Dim thisString As String = "SELECT * FROM PriceProductGroups ORDER BY Id ASC"

            ddlProductGroup.DataSource = settingClass.GetDataTable(thisString)
            ddlProductGroup.DataTextField = "Name"
            ddlProductGroup.DataValueField = "Id"
            ddlProductGroup.DataBind()

            ddlProductGroupProcess.DataSource = settingClass.GetDataTable(thisString)
            ddlProductGroupProcess.DataTextField = "Name"
            ddlProductGroupProcess.DataValueField = "Id"
            ddlProductGroupProcess.DataBind()

            If ddlProductGroup.Items.Count > 0 Then
                ddlProductGroup.Items.Insert(0, New ListItem("", ""))
            End If
            If ddlProductGroupProcess.Items.Count > 0 Then
                ddlProductGroupProcess.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlProductGroup.Items.Clear()
            ddlProductGroupProcess.Items.Clear()
        End Try
    End Sub

    Protected Sub BindPriceGroup()
        ddlPriceGroup.Items.Clear()
        ddlPriceGroupProcess.Items.Clear()
        Try
            Dim thisString As String = "SELECT * FROM PriceGroups ORDER BY Id ASC"

            ddlPriceGroup.DataSource = settingClass.GetDataTable(thisString)
            ddlPriceGroup.DataTextField = "Name"
            ddlPriceGroup.DataValueField = "Id"
            ddlPriceGroup.DataBind()

            ddlPriceGroupProcess.DataSource = settingClass.GetDataTable(thisString)
            ddlPriceGroupProcess.DataTextField = "Name"
            ddlPriceGroupProcess.DataValueField = "Id"
            ddlPriceGroupProcess.DataBind()

            If ddlPriceGroup.Items.Count > 0 Then
                ddlPriceGroup.Items.Insert(0, New ListItem("", ""))
            End If
            If ddlPriceGroupProcess.Items.Count > 0 Then
                ddlPriceGroupProcess.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlPriceGroup.Items.Clear()
            ddlPriceGroupProcess.Items.Clear()
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Process(visible As Boolean, message As String)
        divErrorProcess.Visible = visible : msgErrorProcess.InnerText = message
    End Sub

    Protected Function BindCost(cost As Decimal, priceGroupId As String) As String
        If cost > 0 Then
            If priceGroupId = "2" OrElse priceGroupId = "3" OrElse priceGroupId = "4" OrElse priceGroupId = "5" OrElse priceGroupId = "10" OrElse priceGroupId = "17" OrElse priceGroupId = "19" Then
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
