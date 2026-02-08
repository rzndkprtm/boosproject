Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Bottom_Detail
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/bottom", False)
            Exit Sub
        End If
        If String.IsNullOrEmpty(Request.QueryString("bottomid")) Then
            Response.Redirect("~/setting/specification/bottom", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("bottomid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try

        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnAddColour_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Bottom Colour"

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(lblId.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
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
                    lblIdColour.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Bottom Colour"

                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM BottomColours WHERE Id='" & lblIdColour.Text & "'")
                    If myData Is Nothing Then Exit Sub

                    txtColour.Text = myData("Colour").ToString()
                    txtDescription.Text = myData("Description").ToString()
                    ddlActive.SelectedValue = Convert.ToInt32(myData("Active"))

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
            If Not txtBoeId.Text = "" Then
                If Not IsNumeric(txtBoeId.Text) Then
                    MessageError_Process(True, "BOE ID SHOULD BE NUMERIC !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                    Exit Sub
                End If
            End If
            If txtColour.Text = "" Then
                MessageError_Process(True, "COLOUR IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim name As String = String.Format("{0} {1}", lblName.Text, txtColour.Text.Trim())
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                If lblAction.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM BottomColours ORDER BY Id DESC")
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO BottomColours VALUES (@Id, @BottomId, @BoeId, @Name, @Colour, @Description, @Active)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@BottomId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@BoeId", If(String.IsNullOrEmpty(txtBoeId.Text), CType(DBNull.Value, Object), txtBoeId.Text))
                            myCmd.Parameters.AddWithValue("@Name", name)
                            myCmd.Parameters.AddWithValue("@Colour", txtColour.Text)
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"BottomColours", thisId, Session("LoginId").ToString(), "Bottom Colour Created"}
                    settingClass.Logs(dataLog)

                    url = String.Format("~/setting/specification/bottom/detail?bottomid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If

                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE BottomColours SET BottomId=@BottomId, BoeId=@BoeId, Name=@Name, Colour=@Colour, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblIdColour.Text)
                            myCmd.Parameters.AddWithValue("@BottomId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@BoeId", If(String.IsNullOrEmpty(txtBoeId.Text), CType(DBNull.Value, Object), txtBoeId.Text))
                            myCmd.Parameters.AddWithValue("@Name", name)
                            myCmd.Parameters.AddWithValue("@Colour", txtColour.Text)
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"BottomColours", lblIdColour.Text, Session("LoginId").ToString(), "Bottom Colour Updated"}
                    settingClass.Logs(dataLog)

                    url = String.Format("~/setting/specification/bottom/detail?bottomid={0}", lblId.Text)
                    Response.Redirect(url, False)
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

    Protected Sub btnDeleteColour_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdDeleteColour.Text

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM BottomColours WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='BottomColours' AND DataId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            url = String.Format("~/setting/specification/bottom/detail?bottomid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(bottomId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM Bottoms WHERE Id='" & bottomId & "'")
            If thisData Is Nothing Then
                Response.Redirect("~/setting/specification/bottom", False)
                Exit Sub
            End If

            lblId.Visible = PageAction("Visible ID")
            lblName.Text = thisData("Name").ToString()
            lblDescription.Text = thisData("Description").ToString()
            Dim active As Integer = Convert.ToInt32(thisData("Active"))
            lblActive.Text = "Error"
            If active = 1 Then lblActive.Text = "Yes"
            If active = 0 Then lblActive.Text = "No"

            If Not String.IsNullOrEmpty(bottomId) Then
                Dim designData As DataTable = settingClass.GetDataTable("SELECT Designs.Name AS DesignName FROM Bottoms CROSS APPLY STRING_SPLIT(Bottoms.DesignId, ',') AS splitArray LEFT JOIN Designs ON splitArray.VALUE=Designs.Id WHERE Bottoms.Id='" & bottomId & "' ORDER BY Designs.Id ASC")
                Dim designType As String = String.Empty
                If Not designData.Rows.Count = 0 Then
                    For i As Integer = 0 To designData.Rows.Count - 1
                        Dim designName As String = designData.Rows(i).Item("DesignName").ToString()
                        designType += designName & ", "
                    Next
                End If

                Dim companyData As DataTable = settingClass.GetDataTable("SELECT CompanyDetails.Name AS CompanyName FROM Bottoms CROSS APPLY STRING_SPLIT(Bottoms.CompanyDetailId, ',') AS splitArray LEFT JOIN CompanyDetails ON splitArray.VALUE=CompanyDetails.Id WHERE Bottoms.Id='" & bottomId & "' ORDER BY CompanyDetails.Id ASC")
                Dim companyDetail As String = String.Empty
                If Not companyData.Rows.Count = 0 Then
                    For i As Integer = 0 To companyData.Rows.Count - 1
                        Dim companyDetailName As String = companyData.Rows(i).Item("CompanyName").ToString()
                        companyDetail += companyDetailName & ", "
                    Next
                End If

                lblDesignType.Text = designType.Remove(designType.Length - 2).ToString()
                lblCompanyDetail.Text = companyDetail.Remove(companyDetail.Length - 2).ToString()
            End If

            gvList.DataSource = settingClass.GetDataTable("SELECT *, CASE WHEN Active=1 THEN 'Yes' WHEN Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM BottomColours WHERE BottomId='" & bottomId & "' ORDER BY Colour ASC")
            gvList.DataBind()
            gvList.Columns(1).Visible = PageAction("Visible ID Detail")

            btnAddColour.Visible = PageAction("Add Colour")
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

    Protected Sub MessageError_Process(visible As Boolean, message As String)
        divErrorProcess.Visible = visible : msgErrorProcess.InnerText = message
    End Sub

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
