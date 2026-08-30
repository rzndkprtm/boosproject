Imports System.Data.SqlClient

Partial Class Setting_Price_Surcharge_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price/surcharge", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindDesign()
            BindPriceGroup(ddlDesign.SelectedValue)
            BindFormulaField()
            BindFormulaData(ddlFormulaField.SelectedValue)
            BindFormulaDataB(ddlFormulaFieldB.SelectedValue)

            BindPage(ddlFormulaType.SelectedValue)
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

    Protected Sub ddlFormulaFieldB_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindFormulaDataB(ddlFormulaFieldB.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If txtName.Text = "" Then
                MessageError(True, "SURCHARGE NAME IS REQURIED !")
                Exit Sub
            End If
            If ddlDesign.SelectedValue = "" Then
                MessageError(True, "DESIGN TYPE IS REQUIRED !")
                Exit Sub
            End If
            If ddlPriceGroup.SelectedValue = "" Then
                MessageError(True, "PRICE GROUP IS REQUIRED !")
                Exit Sub
            End If
            If ddlFormulaType.SelectedValue = "" Then
                MessageError(True, "FORMULA TYPE IS REQUIRED !")
                Exit Sub
            End If
            If ddlFormulaType.SelectedValue = "Standard" Then
                If ddlFormulaField.SelectedValue = "" Then
                    MessageError(True, "FORMULA FIELD IS REQUIRED !")
                    Exit Sub
                End If
                If ddlFormulaData.SelectedValue = "" Then
                    MessageError(True, "FORMULA DATA IS REQUIRED !")
                    Exit Sub
                End If

                If ddlFormulaDataB.SelectedValue = "" AndAlso ddlFormulaFieldB.SelectedValue <> "" Then
                    MessageError(True, "FORMULA DATA (ADD) IS REQUIRED !")
                    Exit Sub
                End If
            End If
            If ddlFormulaType.SelectedValue = "Custom" Then
                If txtFormulaCustom.Text = "" Then
                    MessageError(True, "FORMULA IS REQUIRED !")
                    Exit Sub
                End If
            End If
            If txtBuyCharge.Text = "" Then
                MessageError(True, "BUY CHARGE IS REQUIRED !")
                Exit Sub
            End If
            If txtSellCharge.Text = "" Then
                MessageError(True, "SELL CHARGE IS REQUIRED !")
                Exit Sub
            End If
            If txtFactoryCharge.Text = "" Then
                MessageError(True, "FACTORY CHARGE IS REQUIRED !")
                Exit Sub
            End If

            Dim finalFormula As String = String.Format("{0} = {1}", ddlFormulaField.SelectedValue, ddlFormulaData.SelectedValue)
            If Not String.IsNullOrEmpty(ddlFormulaFieldB.SelectedValue) Then
                finalFormula = String.Format("{0} = {1} AND {2} = {3}", ddlFormulaField.SelectedValue, ddlFormulaData.SelectedValue, ddlFormulaFieldB.SelectedValue, ddlFormulaDataB.SelectedValue)
            End If
            If ddlFormulaType.SelectedValue = "Custom" Then
                finalFormula = txtFormulaCustom.Text
            End If

            If msgError.InnerText = "" Then
                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM PriceSurcharges ORDER BY Id DESC")
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO PriceSurcharges VALUES (@Id, @DesignId, @PriceGroupId, @Name, @Type, @Formula, @SellCharge, @BuyCharge, @FactoryCharge, @Description, @Status)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@DesignId", ddlDesign.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@PriceGroupId", ddlPriceGroup.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@Type", ddlFormulaType.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Formula", finalFormula)
                        thisCmd.Parameters.AddWithValue("@SellCharge", txtSellCharge.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@BuyCharge", txtBuyCharge.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@FactoryCharge", txtFactoryCharge.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@Description", descText)
                        thisCmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"PriceSurcharges", thisId, Session("LoginId").ToString(), "Price Surcharge Created"}
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

    Protected Sub BindDesign()
        ddlDesign.Items.Clear()
        Try
            ddlDesign.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM Designs WHERE Active=1 ORDER BY Name ASC")
            ddlDesign.DataTextField = "Name"
            ddlDesign.DataValueField = "Id"
            ddlDesign.DataBind()

            ddlDesign.Items.Insert(0, New ListItem("", ""))
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindPriceGroup(designId As String)
        ddlPriceGroup.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(designId) Then
                Dim type As String = settingClass.GetItemData("SELECT Type FROM Designs WHERE Id='" & designId & "'")
                If Not String.IsNullOrEmpty(type) Then
                    ddlPriceGroup.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM PriceGroups WHERE Type='" & type & "' AND (Status='Active' OR Status='Inactive') ORDER BY Name ASC")
                    ddlPriceGroup.DataTextField = "Name"
                    ddlPriceGroup.DataValueField = "Id"
                    ddlPriceGroup.DataBind()

                    ddlPriceGroup.Items.Insert(0, New ListItem("", ""))
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

                If fieldName = "[Fabric Type]" Then
                    ddlFormulaData.Items.Clear()

                    ddlFormulaData.DataSource = settingClass.GetDataTable("SELECT Id, Name AS DataName FROM Fabrics")
                    ddlFormulaData.DataTextField = "DataName"
                    ddlFormulaData.DataValueField = "Id"
                    ddlFormulaData.DataBind()

                    If ddlFormulaData.Items.Count > 0 Then
                        ddlFormulaData.Items.Insert(0, New ListItem("", ""))
                    End If
                End If

                If fieldName = "[Fabric Colour]" Then
                    ddlFormulaData.Items.Clear()

                    ddlFormulaData.DataSource = settingClass.GetDataTable("SELECT Id, Name AS DataName FROM FabricColours")
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

                If fieldName = "[Fabric Type]" Then
                    ddlFormulaDataB.Items.Clear()

                    ddlFormulaDataB.DataSource = settingClass.GetDataTable("SELECT Id, Name AS DataName FROM Fabrics")
                    ddlFormulaDataB.DataTextField = "DataName"
                    ddlFormulaDataB.DataValueField = "Id"
                    ddlFormulaDataB.DataBind()

                    If ddlFormulaDataB.Items.Count > 0 Then
                        ddlFormulaDataB.Items.Insert(0, New ListItem("", ""))
                    End If
                End If

                If fieldName = "[Fabric Colour]" Then
                    ddlFormulaDataB.Items.Clear()

                    ddlFormulaDataB.DataSource = settingClass.GetDataTable("SELECT Id, Name AS DataName FROM FabricColours")
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