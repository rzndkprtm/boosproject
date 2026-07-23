Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Price_Surcharge_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price/surcharge", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("surchargeid")) Then
            Response.Redirect("~/setting/price/surcharge", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("surchargeid").ToString()

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub ddlDesign_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindPriceGroup(ddlDesign.SelectedValue)
    End Sub

    Protected Sub ddlFormulaType_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindPage(ddlFormulaType.SelectedValue)
    End Sub

    Protected Sub ddlFormulaField_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindFormulaData(ddlFormulaField.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim priceGroup As String = String.Empty
            For Each item As ListItem In lbPriceGroup.Items
                If item.Selected Then
                    priceGroup += item.Value & ","
                End If
            Next
            If priceGroup = "" Then
                MessageError(True, "PRICE GROUP IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Dim finalFormula As String = String.Format("{0} = {1}", ddlFormulaField.SelectedValue, ddlFormulaData.SelectedValue)
                If Not String.IsNullOrEmpty(ddlFormulaFieldB.SelectedValue) Then
                    finalFormula = String.Format("{0} = {1} AND {2} = {3}", ddlFormulaField.SelectedValue, ddlFormulaData.SelectedValue, ddlFormulaFieldB.SelectedValue, ddlFormulaDataB.SelectedValue)
                End If
                If ddlFormulaType.SelectedValue = "Custom" Then
                    finalFormula = txtFormula.Text
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE PriceSurcharges SET DesignId=@DesignId, PriceGroupId=@PriceGroupId, Name=@Name, Type=@Type, Formula=@Formula, BuyCharge=@BuyCharge, SellCharge=@SellCharge, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@DesignId", ddlDesign.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@PriceGroupId", priceGroup)
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@Type", ddlFormulaType.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Formula", finalFormula)
                        thisCmd.Parameters.AddWithValue("@BuyCharge", txtBuyCharge.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@SellCharge", txtSellCharge.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@Description", descText)
                        thisCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"PriceSurcharges", lblId.Text, Session("LoginId").ToString(), "Price Surcharge Updated"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/price/surcharge", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/price/surcharge", False)
    End Sub

    Protected Sub BindData(surchargeId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM PriceSurcharges WHERE Id='" & surchargeId & "'")
            If thisData Is Nothing Then Exit Sub

            Dim designId As String = thisData("DesignId").ToString()
            Dim type As String = thisData("Type").ToString()

            BindDesign()
            BindPriceGroup(designId)
            BindFormulaField()

            txtName.Text = thisData("Name").ToString()
            ddlDesign.SelectedValue = thisData("DesignId").ToString()
            ddlFormulaType.SelectedValue = type
            txtBuyCharge.Text = thisData("BuyCharge").ToString()
            txtSellCharge.Text = thisData("SellCharge").ToString()
            txtDescription.Text = thisData("Description").ToString()
            ddlActive.SelectedValue = Convert.ToInt32(thisData("Active"))

            If Not thisData("PriceGroupId").ToString() = "" Then
                Dim priceGroupArray() As String = thisData("PriceGroupId").ToString().Split(",")
                For Each thisI In priceGroupArray
                    If Not String.IsNullOrEmpty(thisI) Then
                        Dim item = lbPriceGroup.Items.FindByValue(thisI)
                        If item IsNot Nothing Then
                            item.Selected = True
                        End If
                    End If
                Next
            End If

            Dim formula As String = thisData("Formula").ToString()
            Dim conditions = formula.Split(New String() {" AND "}, StringSplitOptions.None)

            Dim i As Integer = 0

            For Each c As String In conditions
                Dim parts = c.Split("="c)
                Dim field As String = parts(0).Trim()

                Dim data As String = parts(1).Trim()
                data = data.Trim("'"c)
                If i = 0 Then
                    BindFormulaData(field)

                    ddlFormulaField.SelectedValue = field
                    ddlFormulaData.SelectedValue = data
                ElseIf i = 1 Then
                    BindFormulaDataB(field)

                    ddlFormulaFieldB.SelectedValue = field
                    ddlFormulaDataB.SelectedValue = data
                End If
                i += 1
            Next

            BindPage(type)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDesign()
        ddlDesign.Items.Clear()
        Try
            ddlDesign.DataSource = settingClass.GetDataTable("SELECT * FROM Designs WHERE Active=1 ORDER BY Name ASC")
            ddlDesign.DataTextField = "Name"
            ddlDesign.DataValueField = "Id"
            ddlDesign.DataBind()

            ddlDesign.Items.Insert(0, New ListItem("", ""))
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindPriceGroup(designId As String)
        lbPriceGroup.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(designId) Then
                Dim type As String = settingClass.GetItemData("SELECT Type FROM Designs WHERE Id='" & designId & "'")
                If Not String.IsNullOrEmpty(type) Then
                    lbPriceGroup.DataSource = settingClass.GetDataTable("SELECT * FROM PriceGroups WHERE Active=1 AND Type='" & type & "' ORDER BY Name ASC")
                    lbPriceGroup.DataTextField = "Name"
                    lbPriceGroup.DataValueField = "Id"
                    lbPriceGroup.DataBind()

                    lbPriceGroup.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindFormulaField()
        ddlFormulaField.Items.Clear()
        ddlFormulaFieldB.Items.Clear()
        Try
            Dim thisQuery As String = "SELECT '[' + COLUMN_NAME + ']' AS FieldNameValue, COLUMN_NAME AS FieldNameText FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'viewSurcharge' AND COLUMN_NAME LIKE '% %' ORDER BY COLUMN_NAME;"

            'If Session("RoleName") = "Developer" Then
            '    thisQuery = "SELECT CASE WHEN COLUMN_NAME LIKE '% %' THEN '[' + COLUMN_NAME + ']' ELSE COLUMN_NAME END AS FieldNameValue, COLUMN_NAME AS FieldNameText FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'viewSurcharge';"
            'End If

            ddlFormulaField.DataSource = settingClass.GetDataTable(thisQuery)
            ddlFormulaField.DataTextField = "FieldNameText"
            ddlFormulaField.DataValueField = "FieldNameValue"
            ddlFormulaField.DataBind()

            ddlFormulaFieldB.DataSource = settingClass.GetDataTable(thisQuery)
            ddlFormulaFieldB.DataTextField = "FieldNameText"
            ddlFormulaFieldB.DataValueField = "FieldNameValue"
            ddlFormulaFieldB.DataBind()

            If ddlFormulaField.Items.Count > 0 Then
                ddlFormulaField.Items.Insert(0, New ListItem("", ""))
            End If
            If ddlFormulaFieldB.Items.Count > 0 Then
                ddlFormulaFieldB.Items.Insert(0, New ListItem("", ""))
            End If

            gvListField.DataSource = settingClass.GetDataTable("SELECT CASE WHEN COLUMN_NAME LIKE '% %' THEN '[' + COLUMN_NAME + ']' ELSE COLUMN_NAME END AS FieldNameValue, COLUMN_NAME AS FieldNameText FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'viewSurcharge';")
            gvListField.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindFormulaData(fieldName As String)
        ddlFormulaData.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(fieldName) Then
                ddlFormulaData.Items.Add(New ListItem("Yes", "'Yes'"))

                If fieldName = "[Blind Type]" Then
                    ddlFormulaData.Items.Clear()

                    ddlFormulaData.DataSource = settingClass.GetDataTable("SELECT Id, Name AS DataName FROM Blinds")
                    ddlFormulaData.DataTextField = "DataName"
                    ddlFormulaData.DataValueField = "Id"
                    ddlFormulaData.DataBind()

                    If ddlFormulaData.Items.Count > 0 Then
                        ddlFormulaData.Items.Insert(0, New ListItem("", ""))
                    End If
                End If

                If fieldName = "[Bottom Type]" Then
                    ddlFormulaData.Items.Clear()

                    ddlFormulaData.DataSource = settingClass.GetDataTable("SELECT Id, Name AS DataName FROM Bottoms")
                    ddlFormulaData.DataTextField = "DataName"
                    ddlFormulaData.DataValueField = "Id"
                    ddlFormulaData.DataBind()

                    If ddlFormulaData.Items.Count > 0 Then
                        ddlFormulaData.Items.Insert(0, New ListItem("", ""))
                    End If
                End If

                If fieldName = "[Control Type]" Then
                    ddlFormulaData.Items.Clear()

                    ddlFormulaData.DataSource = settingClass.GetDataTable("SELECT Id, Name AS DataName FROM ProductControls")
                    ddlFormulaData.DataTextField = "DataName"
                    ddlFormulaData.DataValueField = "Id"
                    ddlFormulaData.DataBind()

                    If ddlFormulaData.Items.Count > 0 Then
                        ddlFormulaData.Items.Insert(0, New ListItem("", ""))
                    End If
                End If

                If fieldName = "[Remote Type]" Then
                    ddlFormulaData.Items.Clear()

                    ddlFormulaData.DataSource = settingClass.GetDataTable("SELECT Id, Name AS DataName FROM Chains WHERE ControlTypeId<>'1'")
                    ddlFormulaData.DataTextField = "DataName"
                    ddlFormulaData.DataValueField = "Id"
                    ddlFormulaData.DataBind()

                    If ddlFormulaData.Items.Count > 0 Then
                        ddlFormulaData.Items.Insert(0, New ListItem("", ""))
                    End If
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindFormulaDataB(fieldName As String)
        ddlFormulaDataB.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(fieldName) Then
                ddlFormulaDataB.Items.Add(New ListItem("Yes", "'Yes'"))

                If fieldName = "[Blind Type]" Then
                    ddlFormulaDataB.Items.Clear()

                    ddlFormulaDataB.DataSource = settingClass.GetDataTable("SELECT Id, Name AS DataName FROM Blinds")
                    ddlFormulaDataB.DataTextField = "DataName"
                    ddlFormulaDataB.DataValueField = "Id"
                    ddlFormulaDataB.DataBind()

                    If ddlFormulaDataB.Items.Count > 0 Then
                        ddlFormulaDataB.Items.Insert(0, New ListItem("", ""))
                    End If
                End If

                If fieldName = "[Bottom Type]" Then
                    ddlFormulaDataB.Items.Clear()

                    ddlFormulaDataB.DataSource = settingClass.GetDataTable("SELECT Id, Name AS DataName FROM Bottoms")
                    ddlFormulaDataB.DataTextField = "DataName"
                    ddlFormulaDataB.DataValueField = "Id"
                    ddlFormulaDataB.DataBind()

                    If ddlFormulaDataB.Items.Count > 0 Then
                        ddlFormulaDataB.Items.Insert(0, New ListItem("", ""))
                    End If
                End If

                If fieldName = "[Control Type]" Then
                    ddlFormulaDataB.Items.Clear()

                    ddlFormulaDataB.DataSource = settingClass.GetDataTable("SELECT Id, Name AS DataName FROM ProductControls")
                    ddlFormulaDataB.DataTextField = "DataName"
                    ddlFormulaDataB.DataValueField = "Id"
                    ddlFormulaDataB.DataBind()

                    If ddlFormulaDataB.Items.Count > 0 Then
                        ddlFormulaDataB.Items.Insert(0, New ListItem("", ""))
                    End If
                End If

                If fieldName = "[Remote Type]" Then
                    ddlFormulaDataB.Items.Clear()

                    ddlFormulaDataB.DataSource = settingClass.GetDataTable("SELECT Id, Name AS DataName FROM Chains WHERE ControlTypeId<>'1'")
                    ddlFormulaDataB.DataTextField = "DataName"
                    ddlFormulaDataB.DataValueField = "Id"
                    ddlFormulaDataB.DataBind()

                    If ddlFormulaDataB.Items.Count > 0 Then
                        ddlFormulaDataB.Items.Insert(0, New ListItem("", ""))
                    End If
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindPage(formulaType As String)
        divFormulaStandard.Visible = False
        divFormulaCustom.Visible = False
        Try
            If formulaType = "Standard" Then
                divFormulaStandard.Visible = True
            End If
            If formulaType = "Custom" Then
                divFormulaCustom.Visible = True
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
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
