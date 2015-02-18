/*globals jQuery, knockout */

(function ($, ko) {
    $.fn.dataFieldMappingGrid = function (settings) {
        var opts = $.extend({}, $.fn.dataFieldMappingGrid.defaultSettings, settings),
            $scope = $(this),
            $hfImportFields = $(settings.hfImportFields),
            $hfInvitationFields = $(settings.hfInvitationFields),
            mappedAltText = opts.mappedAltText,
            unmappedAltText = opts.unmappedAltText;

        function displayMessage(message, cssclass) {
            var messageNode = $("<div/>").addClass('dnnFormMessage ' + cssclass).text(message);
            $scope.append(messageNode);
            messageNode.fadeOut(5000, 'easeInExpo', function () { messageNode.remove(); });
        };

        ko.bindingHandlers['valueWithInit'] = {
            'init': function (element, valueAccessor, allBindingsAccessor) {
                // call existing value init code
                ko.bindingHandlers['value'].init(element, valueAccessor, allBindingsAccessor);
                var modelValue = valueAccessor();
                var elementValue = ko.selectExtensions.readValue(element);
                modelValue(elementValue);
            },
            'update': function (element, valueAccessor) {
                // call existing value update code
                ko.bindingHandlers['value'].update(element, valueAccessor);
            }
        };

        function dataFieldMappingGridViewModel() {
            var self = this;

            function ImportField(item) {
                var self = this;
                self.ImportFieldIndex = item.ImportFieldIndex;
                self.ImportFieldName = item.ImportFieldName;
                self.SkippedField = item.SkippedField;
            }

            function InvitationField(item) {
                var self = this;
                self.FieldIndex = item.FieldIndex;
                self.FieldName = item.FieldName;
                self.LocalizedFieldName = item.LocalizedFieldName;
                self.FieldType = item.FieldType;
                self.FieldWidth = item.FieldWidth;
                self.IsRequired = item.IsRequired;
                self.MappedImportFieldIndex = item.MappedImportFieldIndex;      
            }

            function Crosspoint(importFieldIndex, invitationFieldIndex) {
                var self = this;
                self.ImportFieldIndex = importFieldIndex;
                self.InvitationFieldIndex = invitationFieldIndex;
            }


            var importFields = $.map($.parseJSON($hfImportFields.val()),
                     function (item) {
                         return new ImportField(item)
                     });

            var invitationFieldsJS = $.parseJSON($hfInvitationFields.val());
            var invitationFields = $.map(invitationFieldsJS,
                     function (item) {
                         return new InvitationField(item)
                     });
            self.ImportFields = ko.observableArray(importFields);
            self.InvitationFields = ko.observableArray(invitationFields);

            self.Crosspoints = ko.observableArray([]);
            self.Crosspoints.extend({ rateLimit: 50 });

            self.mappingCSS = function (importFieldIndex, invitationFieldIndex) {
                return ko.computed(function () {
                    var hasCrossPoints = self.Crosspoints().length > 0;
                    var matchedCrosspoint = ko.utils.arrayFirst(self.Crosspoints(), function (item) { return item.ImportFieldIndex == importFieldIndex && item.InvitationFieldIndex == invitationFieldIndex; })
                    return (hasCrossPoints && matchedCrosspoint != null)  ? "Mapped" : "Unmapped";
                }, this)();
            };

            self.toggleMapping = function (importFieldIndex, invitationFieldIndex) {
                var oldCrosspoints = self.Crosspoints.remove(function (item) {
                    return item.InvitationFieldIndex == invitationFieldIndex || item.ImportFieldIndex == importFieldIndex;
                });

                if (oldCrosspoints != null) {

                    // Update the InvitationFields for removed crosspoints
                    ko.utils.arrayForEach(oldCrosspoints, function (item) {
                        var invitationFieldIndex = item.InvitationFieldIndex;
                        self.InvitationFields()[invitationFieldIndex].MappedImportFieldIndex = -1;
                    });
                    self.updatehfInvitationFields();

                    if (ko.utils.arrayFirst(self.Crosspoints(), function (item) { return item.ImportFieldIndex == importFieldIndex; }) != null) return;
                }

                self.Crosspoints.push(new Crosspoint(importFieldIndex, invitationFieldIndex));

                self.InvitationFields()[invitationFieldIndex].MappedImportFieldIndex = importFieldIndex;
                self.updatehfInvitationFields();
            };

            self.updatehfInvitationFields = function () {
                var invitationFieldsJS = {};
                ko.utils.arrayForEach(self.InvitationFields(), function (item) {
                    invitationFieldsJS[item.FieldName] = item;
                });

                var invitationFieldsJSON = ko.toJSON(invitationFieldsJS);

                //var invitationFieldsJSON = ko.toJSON(self.InvitationFields);
                $hfInvitationFields.val(invitationFieldsJSON);
            }

            self.itemOrAltItemCSS = function (index) {
                return index % 2 == 0 ? 'dnnGridItem' : 'dnnGridAltItem';
            }
        }

        var vm = new dataFieldMappingGridViewModel();
        ko.applyBindings(vm, $scope.get(0));
    };

    $.fn.dataFieldMappingGrid.defaultSettings = {
        mappedAltText: 'Mapped',
        unmappedAltText: 'Unmapped'
    };

}(jQuery, ko));