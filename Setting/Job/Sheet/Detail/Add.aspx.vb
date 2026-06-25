Imports System.Data.SqlClient

Partial Class Setting_Job_Sheet_Detail_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/job/sheet/", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("sheetid")) Then
            Response.Redirect("~/setting/job/sheet", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("sheetid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindJobSheet()
            BindViewJob()
            VisibleFormula("1", ddlType.SelectedValue)
            VisibleFormula("2", ddlType2.SelectedValue)
            VisibleFormula("3", ddlType3.SelectedValue)
            VisibleFormula("4", ddlType4.SelectedValue)
            VisibleFormula("5", ddlType5.SelectedValue)
            VisibleFormula("6", ddlType6.SelectedValue)
        End If
    End Sub

    Protected Sub ddlType_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        VisibleFormula("1", ddlType.SelectedValue)
    End Sub

    Protected Sub ddlType2_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        VisibleFormula("2", ddlType2.SelectedValue)
    End Sub

    Protected Sub ddlType3_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        VisibleFormula("3", ddlType3.SelectedValue)
    End Sub

    Protected Sub ddlType4_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        VisibleFormula("4", ddlType4.SelectedValue)
    End Sub

    Protected Sub ddlType5_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        VisibleFormula("5", ddlType5.SelectedValue)
    End Sub

    Protected Sub ddlType6_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        VisibleFormula("6", ddlType6.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If msgError.InnerText = "" Then
                Dim formula As String = ddlFormula.SelectedValue
                If ddlType.SelectedValue = "Custom" Then formula = txtFormula.Text

                Dim formula2 As String = ddlFormula2.SelectedValue
                If ddlType2.SelectedValue = "Custom" Then formula2 = txtFormula2.Text

                Dim formula3 As String = ddlFormula3.SelectedValue
                If ddlType3.SelectedValue = "Custom" Then formula3 = txtFormula3.Text

                Dim formula4 As String = ddlFormula4.SelectedValue
                If ddlType4.SelectedValue = "Custom" Then formula4 = txtFormula4.Text

                Dim formula5 As String = ddlFormula5.SelectedValue
                If ddlType5.SelectedValue = "Custom" Then formula5 = txtFormula5.Text

                Dim formula6 As String = ddlFormula6.SelectedValue
                If ddlType6.SelectedValue = "Custom" Then formula6 = txtFormula6.Text

                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM JobSheetDetails ORDER BY Id DESC")
                Dim sortOrder As String = settingClass.CreateId("SELECT TOP 1 SortOrder FROM JobSheetDetails WHERE JobSheetId='" & lblId.Text & "' ORDER BY Id DESC")

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO JobSheetDetails VALUES (@Id, @JobSheetId, @Name, @Type1, @Formula1, @Type2, @Formula2, @Type3, @Formula3, @Type4, @Formula4, @Type5, @Formula5, @Type6, @Formula6, NULL, NULL, NULL, NULL, @SortOrder, NULL, 1)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@JobSheetId", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text)
                        thisCmd.Parameters.AddWithValue("@Type1", ddlType.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Type2", ddlType2.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Type3", ddlType3.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Type4", ddlType4.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Type5", ddlType5.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Type6", ddlType6.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Formula1", formula)
                        thisCmd.Parameters.AddWithValue("@Formula2", formula2)
                        thisCmd.Parameters.AddWithValue("@Formula3", formula3)
                        thisCmd.Parameters.AddWithValue("@Formula4", formula4)
                        thisCmd.Parameters.AddWithValue("@Formula5", formula5)
                        thisCmd.Parameters.AddWithValue("@Formula6", formula6)
                        thisCmd.Parameters.AddWithValue("@SortOrder", sortOrder)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"JobSheetDetails", thisId, Session("LoginId").ToString(), "Job Sheet Detail Created"}
                settingClass.Logs(dataLog)

                url = String.Format("~/setting/job/sheet/detail/?sheetid={0}", lblId.Text)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        url = String.Format("~/setting/job/sheet/detail/?sheetid={0}", lblId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub BindJobSheet()
        ddlJobSheet.Items.Clear()
        Try
            ddlJobSheet.DataSource = settingClass.GetDataTable("SELECT * FROM JobSheets ORDER BY Name ASC")
            ddlJobSheet.DataTextField = "Name"
            ddlJobSheet.DataValueField = "Id"
            ddlJobSheet.DataBind()

            ddlJobSheet.SelectedValue = lblId.Text
            ddlJobSheet.Enabled = False
        Catch ex As Exception
            ddlJobSheet.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub BindViewJob()
        ddlFormula.Items.Clear() : ddlFormula2.Items.Clear() : ddlFormula3.Items.Clear() : ddlFormula4.Items.Clear() : ddlFormula5.Items.Clear() : ddlFormula6.Items.Clear()
        Try
            Dim thisString As String = "SELECT COLUMN_NAME AS FieldName FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=N'viewJob'"

            ddlFormula.DataSource = settingClass.GetDataTable(thisString)
            ddlFormula.DataTextField = "FieldName"
            ddlFormula.DataValueField = "FieldName"
            ddlFormula.DataBind()

            ddlFormula2.DataSource = settingClass.GetDataTable(thisString)
            ddlFormula2.DataTextField = "FieldName"
            ddlFormula2.DataValueField = "FieldName"
            ddlFormula2.DataBind()

            ddlFormula3.DataSource = settingClass.GetDataTable(thisString)
            ddlFormula3.DataTextField = "FieldName"
            ddlFormula3.DataValueField = "FieldName"
            ddlFormula3.DataBind()

            ddlFormula4.DataSource = settingClass.GetDataTable(thisString)
            ddlFormula4.DataTextField = "FieldName"
            ddlFormula4.DataValueField = "FieldName"
            ddlFormula4.DataBind()

            ddlFormula5.DataSource = settingClass.GetDataTable(thisString)
            ddlFormula5.DataTextField = "FieldName"
            ddlFormula5.DataValueField = "FieldName"
            ddlFormula5.DataBind()

            ddlFormula6.DataSource = settingClass.GetDataTable(thisString)
            ddlFormula6.DataTextField = "FieldName"
            ddlFormula6.DataValueField = "FieldName"
            ddlFormula6.DataBind()

            If ddlFormula.Items.Count > 0 Then
                ddlFormula.Items.Insert(0, New ListItem("", ""))
            End If
            If ddlFormula2.Items.Count > 0 Then
                ddlFormula2.Items.Insert(0, New ListItem("", ""))
            End If
            If ddlFormula3.Items.Count > 0 Then
                ddlFormula3.Items.Insert(0, New ListItem("", ""))
            End If
            If ddlFormula4.Items.Count > 0 Then
                ddlFormula4.Items.Insert(0, New ListItem("", ""))
            End If
            If ddlFormula5.Items.Count > 0 Then
                ddlFormula5.Items.Insert(0, New ListItem("", ""))
            End If
            If ddlFormula6.Items.Count > 0 Then
                ddlFormula6.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlFormula.Items.Clear() : ddlFormula2.Items.Clear() : ddlFormula3.Items.Clear() : ddlFormula4.Items.Clear() : ddlFormula5.Items.Clear() : ddlFormula6.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub VisibleFormula(number As String, type As String)
        Try
            If number = "1" Then
                divFormulaField.Visible = False : divFormulaCustom.Visible = False
                If type = "Field" Then
                    divFormulaField.Visible = True : divFormulaCustom.Visible = False
                End If
                If type = "Custom" Then
                    divFormulaField.Visible = False : divFormulaCustom.Visible = True
                End If
            End If
            If number = "2" Then
                divFormulaField2.Visible = False : divFormulaCustom2.Visible = False
                If type = "Field" Then
                    divFormulaField2.Visible = True : divFormulaCustom2.Visible = False
                End If
                If type = "Custom" Then
                    divFormulaField2.Visible = False : divFormulaCustom2.Visible = True
                End If
            End If
            If number = "3" Then
                divFormulaField3.Visible = False : divFormulaCustom3.Visible = False
                If type = "Field" Then
                    divFormulaField3.Visible = True : divFormulaCustom3.Visible = False
                End If
                If type = "Custom" Then
                    divFormulaField3.Visible = False : divFormulaCustom3.Visible = True
                End If
            End If
            If number = "4" Then
                divFormulaField4.Visible = False : divFormulaCustom4.Visible = False
                If type = "Field" Then
                    divFormulaField4.Visible = True : divFormulaCustom4.Visible = False
                End If
                If type = "Custom" Then
                    divFormulaField4.Visible = False : divFormulaCustom4.Visible = True
                End If
            End If
            If number = "5" Then
                divFormulaField5.Visible = False : divFormulaCustom5.Visible = False
                If type = "Field" Then
                    divFormulaField5.Visible = True : divFormulaCustom5.Visible = False
                End If
                If type = "Custom" Then
                    divFormulaField5.Visible = False : divFormulaCustom5.Visible = True
                End If
            End If
            If number = "6" Then
                divFormulaField6.Visible = False : divFormulaCustom6.Visible = False
                If type = "Field" Then
                    divFormulaField6.Visible = True : divFormulaCustom6.Visible = False
                End If
                If type = "Custom" Then
                    divFormulaField6.Visible = False : divFormulaCustom6.Visible = True
                End If
            End If
        Catch ex As Exception
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
