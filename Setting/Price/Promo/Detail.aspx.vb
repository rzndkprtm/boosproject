Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Partial Class Setting_Price_Promo_Detail
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty
    Dim enUS As CultureInfo = New CultureInfo("en-US")

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price/promo", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("promoid")) Then
            Response.Redirect("~/setting/price/promo", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("promoid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            MessageError_Process(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnAddDetail_Click(sender As Object, e As EventArgs)
        MessageError_ProcessDetail(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessDetail(); };"
        Try
            lblAction.Text = "Add"
            titleProcessDetail.InnerText = "Add Promo Detail"
            ddlPromoType.Enabled = True

            BindDesignPromo()
            BindBlindPromo()
            BindFabricPromo()
            BindFabricColourPromo()

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDetail", thisScript, True)
        Catch ex As Exception
            MessageError_ProcessDetail(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ProcessDetail(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDetail", thisScript, True)
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim promoDetailId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_ProcessDetail(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcessDetail(); };"
                Try
                    lblIdDetail.Text = promoDetailId
                    lblAction.Text = "Edit"
                    titleProcessDetail.InnerText = "Detail Promo Detail"

                    Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM PromoDetails WHERE Id='" & promoDetailId & "'")
                    If thisData Is Nothing Then Exit Sub

                    BindDesignPromo(True)
                    BindBlindPromo(True)
                    BindFabricPromo(True)
                    BindFabricColourPromo(True)

                    Dim type As String = thisData("Type").ToString()
                    Dim dataId As String = thisData("DataId").ToString()

                    ddlPromoType.SelectedValue = type
                    ddlPromoType.Enabled = False
                    If type = "Designs" Then ddlDesignPromo.SelectedValue = dataId
                    If type = "Blinds" Then ddlBlindPromo.SelectedValue = dataId
                    If type = "Fabrics" Then ddlFabricPromo.SelectedValue = dataId
                    If type = "FabricColours" Then ddlFabricColourPromo.SelectedValue = dataId
                    txtDiscount.Text = Convert.ToDecimal(thisData("Discount")).ToString("G29", enUS)

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDetail", thisScript, True)
                Catch ex As Exception
                    MessageError_ProcessDetail(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_ProcessDetail(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDetail", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If txtName.Text = "" Then
                MessageError_Process(True, "COMPANY NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE Promos SET Name=@Name, StartDate=@StartDate, EndDate=@EndDate, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@StartDate", txtStartDate.Text)
                        myCmd.Parameters.AddWithValue("@EndDate", txtEndDate.Text)
                        myCmd.Parameters.AddWithValue("@Description", descText)
                        myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"Promos", lblId.Text, Session("LoginId").ToString(), "Promo Updated"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/price/promo/detail", False)
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnProcessDetail_Click(sender As Object, e As EventArgs)
        MessageError_ProcessDetail(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessDetail(); };"
        Try
            If ddlPromoType.SelectedValue = "" Then
                MessageError_ProcessDetail(True, "PROMO TYPE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDetail", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcessDetail.InnerText = "" Then
                Dim dataId As String = String.Empty
                If ddlPromoType.SelectedValue = "Designs" Then dataId = ddlDesignPromo.SelectedValue
                If ddlPromoType.SelectedValue = "Blinds" Then dataId = ddlBlindPromo.SelectedValue
                If ddlPromoType.SelectedValue = "Fabrics" Then dataId = ddlFabricPromo.SelectedValue
                If ddlPromoType.SelectedValue = "FabricColours" Then dataId = ddlFabricColourPromo.SelectedValue

                If lblAction.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM PromoDetails ORDER BY Id DESC")

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO PromoDetails VALUES (@Id, @PromoId, @Type, @DataId, @Discount)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@PromoId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@Type", ddlPromoType.SelectedValue)
                            myCmd.Parameters.AddWithValue("@DataId", dataId)
                            myCmd.Parameters.AddWithValue("@Discount", txtDiscount.Text)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"PromoDetails", thisId, Session("LoginId").ToString(), "Promo Detail Created"}
                    settingClass.Logs(dataLog)

                    url = String.Format("~/setting/price/promo/detail?promoid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If

                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE PromoDetails SET Type=@Type, DataId=@DataId, Discount=@Discount WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblIdDetail.Text)
                            myCmd.Parameters.AddWithValue("@Type", ddlPromoType.SelectedValue)
                            myCmd.Parameters.AddWithValue("@DataId", dataId)
                            myCmd.Parameters.AddWithValue("@Discount", txtDiscount.Text)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"PromoDetails", lblIdDetail.Text, Session("LoginId").ToString(), "Promo Detail Updated"}
                    settingClass.Logs(dataLog)

                    url = String.Format("~/setting/price/promo/detail?promoid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If
            End If
        Catch ex As Exception
            MessageError_ProcessDetail(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ProcessDetail(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDetail", thisScript, True)
        End Try
    End Sub

    Protected Sub btnDeleteDetail_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim dataId As String = txtIdDeleteDetail.Text

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM PromoDetails WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", dataId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='PromoDetails' AND DataId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", dataId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            url = String.Format("~/setting/price/promo/detail?promoid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(promoId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM Promos WHERE Id='" & promoId & "'")
            If thisData Is Nothing Then
                Response.Redirect("~/setting/price/promo", False)
                Exit Sub
            End If

            lblName.Text = thisData("Name").ToString()
            txtName.Text = thisData("Name").ToString()
            lblStartDate.Text = Convert.ToDateTime(thisData("StartDate")).ToString("dd MMM yyyy")
            txtStartDate.Text = Convert.ToDateTime(thisData("StartDate")).ToString("yyyy-MM-dd")
            lblEndDate.Text = Convert.ToDateTime(thisData("EndDate")).ToString("dd MMM yyyy")
            txtEndDate.Text = Convert.ToDateTime(thisData("EndDate")).ToString("yyyy-MM-dd")
            lblDescription.Text = thisData("Description").ToString()
            txtDescription.Text = thisData("Description").ToString()

            Dim active As Integer = Convert.ToInt32(thisData("Active"))
            lblActive.Text = "Error"
            If active = 1 Then lblActive.Text = "Yes"
            If active = 0 Then lblActive.Text = "No"
            ddlActive.SelectedValue = active

            gvList.DataSource = settingClass.GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")
            gvList.DataBind()
            gvList.Columns(1).Visible = PageAction("Visible ID Detail")

            btnAddDetail.Visible = PageAction("Add Detail")
            aEdit.Visible = PageAction("Edit")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not IsPostBack Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDesignPromo(Optional isEdit As Boolean = False)
        ddlDesignPromo.Items.Clear()
        Try
            Dim thisString As String = "SELECT * FROM Designs WHERE Active=1 ORDER BY Name ASC"
            If isEdit = True Then
                thisString = "SELECT * FROM Designs ORDER BY Name ASC"
            End If
            ddlDesignPromo.DataSource = settingClass.GetDataTable(thisString)
            ddlDesignPromo.DataTextField = "Name"
            ddlDesignPromo.DataValueField = "Id"
            ddlDesignPromo.DataBind()
            If ddlDesignPromo.Items.Count > 1 Then
                ddlDesignPromo.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlDesignPromo.Items.Clear()
        End Try
    End Sub

    Protected Sub BindBlindPromo(Optional isEdit As Boolean = False)
        ddlBlindPromo.Items.Clear()
        Try
            Dim thisString As String = "SELECT Blinds.*, CONVERT(VARCHAR, Designs.Name) + ' | ' + CONVERT(VARCHAR, Blinds.Name) AS BlindName FROM Blinds LEFT JOIN Designs ON Blinds.DesignId = Designs.Id WHERE Blinds.Active=1 AND Designs.Active=1 ORDER BY Designs.Name, Blinds.Name ASC"
            If isEdit = True Then
                thisString = "SELECT Blinds.*, CONVERT(VARCHAR, Designs.Name) + ' | ' + CONVERT(VARCHAR, Blinds.Name) AS BlindName FROM Blinds LEFT JOIN Designs ON Blinds.DesignId = Designs.Id ORDER BY Designs.Name, Blinds.Name ASC"
            End If
            ddlBlindPromo.DataSource = settingClass.GetDataTable(thisString)
            ddlBlindPromo.DataTextField = "BlindName"
            ddlBlindPromo.DataValueField = "Id"
            ddlBlindPromo.DataBind()
            If ddlBlindPromo.Items.Count > 1 Then
                ddlBlindPromo.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlBlindPromo.Items.Clear()
        End Try
    End Sub

    Protected Sub BindFabricPromo(Optional isEdit As Boolean = False)
        ddlFabricPromo.Items.Clear()
        Try
            Dim thisString As String = "SELECT * FROM Fabrics WHERE Active=1 ORDER BY Name ASC"
            If isEdit = True Then
                thisString = "SELECT * FROM Fabrics ORDER BY Name ASC"
            End If
            ddlFabricPromo.DataSource = settingClass.GetDataTable(thisString)
            ddlFabricPromo.DataTextField = "Name"
            ddlFabricPromo.DataValueField = "Id"
            ddlFabricPromo.DataBind()
            If ddlFabricPromo.Items.Count > 1 Then
                ddlFabricPromo.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlFabricPromo.Items.Clear()
        End Try
    End Sub

    Protected Sub BindFabricColourPromo(Optional isEdit As Boolean = False)
        ddlFabricColourPromo.Items.Clear()
        Try
            Dim thisString As String = "SELECT * FROM FabricColours WHERE Active=1 ORDER BY Name ASC"
            If isEdit = True Then
                thisString = "SELECT * FROM FabricColours ORDER BY Name ASC"
            End If
            ddlFabricColourPromo.DataSource = settingClass.GetDataTable(thisString)
            ddlFabricColourPromo.DataTextField = "Name"
            ddlFabricColourPromo.DataValueField = "Id"
            ddlFabricColourPromo.DataBind()
            If ddlFabricColourPromo.Items.Count > 1 Then
                ddlFabricColourPromo.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlFabricColourPromo.Items.Clear()
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Process(visible As Boolean, message As String)
        divErrorProcess.Visible = visible : msgErrorProcess.InnerText = message
    End Sub

    Protected Sub MessageError_ProcessDetail(visible As Boolean, message As String)
        divErrorProcessDetail.Visible = visible : msgErrorProcessDetail.InnerText = message
    End Sub

    Protected Function DiscountTitle(type As String, dataId As String) As String
        If String.IsNullOrEmpty(type) Then Return String.Empty
        Return settingClass.GetItemData(String.Format("SELECT Name FROM {0} WHERE Id='{1}'", type, dataId))
    End Function

    Protected Function DiscountValue(data As Decimal) As String
        If data > 0 Then
            Return data.ToString("G29", enUS) & "%"
        End If
        Return "ERROR"
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
