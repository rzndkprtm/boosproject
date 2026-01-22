Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Product_Detail
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/product", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("productid")) Then
            Response.Redirect("~/setting/specification/product", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("productid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnEditProduct_Click(sender As Object, e As EventArgs)
        url = String.Format("~/setting/specification/product/edit?id={0}", lblId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnAddKit_Click(sender As Object, e As EventArgs)
        MessageError_ProcessKit(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessKit(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Product Kit"

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessKit", thisScript, True)
        Catch ex As Exception
            MessageError_ProcessKit(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessKit", thisScript, True)
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()

            If e.CommandName = "Detail" Then
                MessageError_ProcessKit(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcessKit(); };"
                Try
                    lblIdKit.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Product Kit"

                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM ProductKits WHERE Id='" & lblIdKit.Text & "'")
                    If myData Is Nothing Then Exit Sub

                    txtKitId.Text = myData("KitId").ToString()
                    txtVenId.Text = myData("VenId").ToString()
                    txtCustomName.Text = myData("Name").ToString().Replace(txtNameKit.Text, "").Trim()
                    ddlBlindStatus.SelectedValue = myData("BlindStatus").ToString()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessKit", thisScript, True)
                Catch ex As Exception
                    MessageError_ProcessKit(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessKit", thisScript, True)
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingClass.GetDataTable("SELECT * FROM Logs WHERE DataId='" & dataId & "' AND Type='ProductKits' ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnProcessKit_Click(sender As Object, e As EventArgs)
        MessageError_ProcessKit(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessKit(); };"
        Try
            If Not String.IsNullOrEmpty(txtKitId.Text) Then
                If Not IsNumeric(txtKitId.Text) Then
                    MessageError_ProcessKit(True, "KIT ID SHOULD BE NUMERIC !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessKit", thisScript, True)
                End If
            End If

            If Not String.IsNullOrEmpty(txtVenId.Text) Then
                If Not IsNumeric(txtVenId.Text) Then
                    MessageError_ProcessKit(True, "VEN ID SHOULD BE NUMERIC !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessKit", thisScript, True)
                End If
            End If

            If msgErrorProcessKit.InnerText = "" Then
                Dim kitName As String = txtNameKit.Text.Trim() & " " & txtCustomName.Text.Trim()

                If lblAction.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM ProductKits ORDER BY Id DESC")

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO ProductKits VALUES (@Id, @ProductId, @KitId, @VenId, @Name, @Status)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@ProductId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(txtKitId.Text), CType(DBNull.Value, Object), txtKitId.Text))
                            myCmd.Parameters.AddWithValue("@VenId", If(txtVenId.Text = String.Empty, CType(DBNull.Value, Object), txtVenId.Text))
                            myCmd.Parameters.AddWithValue("@Name", kitName)
                            myCmd.Parameters.AddWithValue("@Status", ddlBlindStatus.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"ProductKits", thisId, Session("LoginId").ToString(), "HardwareKit Created"}
                    settingClass.Logs(dataLog)

                    url = String.Format("~/setting/specification/product/detail?productid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If
                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE ProductKits SET KitId=@KitId, VenId=@VenId, Name=@Name, BlindStatus=@Status WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblIdKit.Text)
                            myCmd.Parameters.AddWithValue("@ProductId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@KitId", If(String.IsNullOrEmpty(txtKitId.Text), CType(DBNull.Value, Object), txtKitId.Text))
                            myCmd.Parameters.AddWithValue("@VenId", If(txtVenId.Text = String.Empty, CType(DBNull.Value, Object), txtVenId.Text))
                            myCmd.Parameters.AddWithValue("@Name", kitName)
                            myCmd.Parameters.AddWithValue("@Status", ddlBlindStatus.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"ProductKits", lblIdKit.Text, Session("LoginId").ToString(), "HardwareKit Updated"}
                    settingClass.Logs(dataLog)

                    url = String.Format("~/setting/specification/product/detail?productid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If
            End If
        Catch ex As Exception
            MessageError_ProcessKit(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessKit", thisScript, True)
        End Try
    End Sub

    Protected Sub btnDeleteKit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdDeleteKit.Text

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM ProductKits WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='ProductKits' AND DataId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            url = String.Format("~/setting/specification/product/detail?productid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindData(productId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT Products.*, Designs.Name AS DesignName, Blinds.Name AS BlindName, ProductTubes.Name AS TubeName, ProductControls.Name AS ControlName, ProductColours.Name AS ColourName FROM Products LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductTubes ON Products.TubeType=ProductTubes.Id LEFT JOIN ProductControls ON Products.ControlType=ProductControls.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE Products.Id='" & productId & "'")
            If thisData Is Nothing Then
                Response.Redirect("~/setting/specification/product/", False)
                Exit Sub
            End If

            lblName.Text = thisData("Name").ToString()
            lblInvoiceName.Text = thisData("InvoiceName").ToString()
            txtNameKit.Text = thisData("Name").ToString()
            lblDesignName.Text = thisData("DesignName").ToString()
            lblBlindName.Text = thisData("BlindName").ToString()
            lblTubeType.Text = thisData("TubeName").ToString()
            lblControlType.Text = thisData("ControlName").ToString()
            lblColourType.Text = thisData("ColourName").ToString()
            lblDescription.Text = thisData("Description").ToString()

            Dim company As String = thisData("CompanyDetailId").ToString()
            If Not String.IsNullOrEmpty(company) Then
                Dim myData As DataTable = settingClass.GetDataTable("SELECT CompanyDetails.Name AS CompanyName FROM Products CROSS APPLY STRING_SPLIT(Products.CompanyDetailId, ',') AS splitArray LEFT JOIN CompanyDetails ON splitArray.VALUE=CompanyDetails.Id WHERE Products.Id='" & productId & "' ORDER BY CompanyDetails.Id ASC")
                Dim hasil As String = String.Empty
                If Not myData.Rows.Count = 0 Then
                    For i As Integer = 0 To myData.Rows.Count - 1
                        Dim designName As String = myData.Rows(i)("CompanyName").ToString()
                        hasil += designName & ", "
                    Next
                End If
                lblCompanyName.Text = hasil.Remove(hasil.Length - 2).ToString()
            End If

            Dim active As Integer = Convert.ToInt32(thisData("Active"))
            If active = 1 Then lblActive.Text = "Yes"
            If active = 0 Then lblActive.Text = "No"

            gvList.DataSource = settingClass.GetDataTable("SELECT * FROM ProductKits WHERE ProductId='" & productId & "'")
            gvList.DataBind()

            btnEditProduct.Visible = PageAction("Edit")
            btnAddKit.Visible = PageAction("Add Kit")
        Catch ex As Exception
            MessageError(True, ex.ToString)
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_ProcessKit(visible As Boolean, message As String)
        divErrorProcessKit.Visible = visible : msgErrorProcessKit.InnerText = message
    End Sub

    Protected Sub MessageError_Log(Svisiblehow As Boolean, message As String)
        divErrorLog.Visible = Visible : msgErrorLog.InnerText = message
    End Sub

    Protected Function BindTextLog(logId As String) As String
        Return settingClass.getTextLog(logId)
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
