function ULSPjj(){var o=new Object;o.ULSTeamName="DLC Server";o.ULSFileName="SP.DocumentManagement.js";return o;}
Type.registerNamespace("SP.DocumentManagement");SP.DocumentManagement.MetadataDefaults=function(a){ULSPjj:;SP.DocumentManagement.MetadataDefaults.initializeBase(this,[a,SP.ClientUtility.getOrCreateObjectPathForConstructor(a,"{9410c048-d7df-4e86-b218-be5b4f4c427f}",arguments)])};SP.DocumentManagement.MetadataDefaults.newObject=function(a,b){ULSPjj:;return new SP.DocumentManagement.MetadataDefaults(a,new SP.ObjectPathConstructor(a,"{9410c048-d7df-4e86-b218-be5b4f4c427f}",[b]))};SP.DocumentManagement.MetadataDefaults.prototype={setFieldDefault:function(e,d,f){ULSPjj:;var b=this.get_context(),a,c=new SP.ClientActionInvokeMethod(this,"SetFieldDefault",[e,d,f]);b.addQuery(c);a=new SP.BooleanResult;b.addQueryIdAndResultObject(c.get_id(),a);return a},update:function(){ULSPjj:;var a=this.get_context(),b=new SP.ClientActionInvokeMethod(this,"Update",null);a.addQuery(b)}};Type.registerNamespace("SP.DocumentSet");SP.DocumentSet.AllowedContentTypeCollection=function(b,a){ULSPjj:;SP.DocumentSet.AllowedContentTypeCollection.initializeBase(this,[b,a])};SP.DocumentSet.AllowedContentTypeCollection.prototype={itemAt:function(a){ULSPjj:;return this.getItemAtIndex(a)},get_item:function(a){ULSPjj:;return this.getItemAtIndex(a)},get_childItemType:function(){ULSPjj:;return SP.ContentTypeId},add:function(c){ULSPjj:;var a=this.get_context(),b=new SP.ClientActionInvokeMethod(this,"Add",[c]);a.addQuery(b)},remove:function(c){ULSPjj:;var a=this.get_context(),b=new SP.ClientActionInvokeMethod(this,"Remove",[c]);a.addQuery(b)}};SP.DocumentSet.DefaultDocument=function(b,a){ULSPjj:;SP.DocumentSet.DefaultDocument.initializeBase(this,[b,a])};SP.DocumentSet.DefaultDocument.prototype={get_contentTypeId:function(){ULSPjj:;this.checkUninitializedProperty("ContentTypeId");return this.get_objectData().get_properties().ContentTypeId},set_contentTypeId:function(a){ULSPjj:;this.get_objectData().get_properties().ContentTypeId=a;this.get_context()&&this.get_context().addQuery(new SP.ClientActionSetProperty(this,"ContentTypeId",a));return a},get_documentPath:function(){ULSPjj:;this.checkUninitializedProperty("DocumentPath");return this.get_objectData().get_properties().DocumentPath},get_name:function(){ULSPjj:;this.checkUninitializedProperty("Name");return this.get_objectData().get_properties().Name},initPropertiesFromJson:function(b){ULSPjj:;SP.ClientObject.prototype.initPropertiesFromJson.call(this,b);var a;a=b.ContentTypeId;if(!SP.ScriptUtility.isUndefined(a)){this.get_objectData().get_properties().ContentTypeId=SP.DataConvert.fixupType(this.get_context(),a);delete b.ContentTypeId}a=b.DocumentPath;if(!SP.ScriptUtility.isUndefined(a)){this.get_objectData().get_properties().DocumentPath=SP.DataConvert.fixupType(this.get_context(),a);delete b.DocumentPath}a=b.Name;if(!SP.ScriptUtility.isUndefined(a)){this.get_objectData().get_properties().Name=a;delete b.Name}}};SP.DocumentSet.DefaultDocumentPropertyNames=function(){};SP.DocumentSet.DefaultDocumentCollection=function(b,a){ULSPjj:;SP.DocumentSet.DefaultDocumentCollection.initializeBase(this,[b,a])};SP.DocumentSet.DefaultDocumentCollection.prototype={itemAt:function(a){ULSPjj:;return this.getItemAtIndex(a)},get_item:function(a){ULSPjj:;return this.getItemAtIndex(a)},get_childItemType:function(){ULSPjj:;return SP.DocumentSet.DefaultDocument},add:function(e,d,c){ULSPjj:;var a=this.get_context(),b;b=new SP.DocumentSet.DefaultDocument(a,new SP.ObjectPathMethod(a,this.get_path(),"Add",[e,d,c]));return b},changeContentTypeForDocument:function(e,d){ULSPjj:;var b=this.get_context(),a,c=new SP.ClientActionInvokeMethod(this,"ChangeContentTypeForDocument",[e,d]);b.addQuery(c);a=new SP.BooleanResult;b.addQueryIdAndResultObject(c.get_id(),a);return a},remove:function(c){ULSPjj:;var a=this.get_context(),b=new SP.ClientActionInvokeMethod(this,"Remove",[c]);a.addQuery(b)}};SP.DocumentSet.DocumentSet=function(b,a){ULSPjj:;SP.DocumentSet.DocumentSet.initializeBase(this,[b,a])};SP.DocumentSet.DocumentSet.getDocumentSet=function(a,c){ULSPjj:;if(!a)throw Error.argumentNull("context");var b;b=new SP.DocumentSet.DocumentSet(a,new SP.ObjectPathStaticMethod(a,"{e32a87f7-b866-407d-971d-027ed940d50f}","GetDocumentSet",[c]));return b};SP.DocumentSet.DocumentSet.create=function(a,d,f,e){ULSPjj:;if(!a)throw Error.argumentNull("context");var b,c=new SP.ClientActionInvokeStaticMethod(a,"{e32a87f7-b866-407d-971d-027ed940d50f}","Create",[d,f,e]);a.addQuery(c);b=new SP.StringResult;a.addQueryIdAndResultObject(c.get_id(),b);return b};SP.DocumentSet.DocumentSet.importDocumentSet=function(a,d,f,e,c){ULSPjj:;if(!a)throw Error.argumentNull("context");var b;b=new SP.DocumentSet.DocumentSet(a,new SP.ObjectPathStaticMethod(a,"{e32a87f7-b866-407d-971d-027ed940d50f}","ImportDocumentSet",[d,f,e,c]));return b};SP.DocumentSet.DocumentSet.prototype={exportDocumentSet:function(){ULSPjj:;var b=this.get_context(),a,c=new SP.ClientActionInvokeMethod(this,"ExportDocumentSet",null);b.addQuery(c);a=[];b.addQueryIdAndResultObject(c.get_id(),a);return a}};SP.DocumentSet.DocumentSetTemplate=function(b,a){ULSPjj:;SP.DocumentSet.DocumentSetTemplate.initializeBase(this,[b,a])};SP.DocumentSet.DocumentSetTemplate.getContentTypeId=function(a){ULSPjj:;if(!a)throw Error.argumentNull("context");var b,c=new SP.ClientActionInvokeStaticMethod(a,"{1554af8c-7213-418c-a4a8-b06e7603c68a}","GetContentTypeId",null);a.addQuery(c);b=new SP.ContentTypeId;a.addQueryIdAndResultObject(c.get_id(),b);return b};SP.DocumentSet.DocumentSetTemplate.getDocumentSetTemplate=function(a,c){ULSPjj:;if(!a)throw Error.argumentNull("context");var b;b=new SP.DocumentSet.DocumentSetTemplate(a,new SP.ObjectPathStaticMethod(a,"{1554af8c-7213-418c-a4a8-b06e7603c68a}","GetDocumentSetTemplate",[c]));return b};SP.DocumentSet.DocumentSetTemplate.isChildOfDocumentSetContentType=function(a,d){ULSPjj:;if(!a)throw Error.argumentNull("context");var b,c=new SP.ClientActionInvokeStaticMethod(a,"{1554af8c-7213-418c-a4a8-b06e7603c68a}","IsChildOfDocumentSetContentType",[d]);a.addQuery(c);b=new SP.BooleanResult;a.addQueryIdAndResultObject(c.get_id(),b);return b};SP.DocumentSet.DocumentSetTemplate.prototype={get_allowedContentTypes:function(){ULSPjj:;var a=this.get_objectData().get_clientObjectProperties().AllowedContentTypes;if(SP.ScriptUtility.isUndefined(a)){a=new SP.DocumentSet.AllowedContentTypeCollection(this.get_context(),new SP.ObjectPathProperty(this.get_context(),this.get_path(),"AllowedContentTypes"));this.get_objectData().get_clientObjectProperties().AllowedContentTypes=a}return a},get_defaultDocuments:function(){ULSPjj:;var a=this.get_objectData().get_clientObjectProperties().DefaultDocuments;if(SP.ScriptUtility.isUndefined(a)){a=new SP.DocumentSet.DefaultDocumentCollection(this.get_context(),new SP.ObjectPathProperty(this.get_context(),this.get_path(),"DefaultDocuments"));this.get_objectData().get_clientObjectProperties().DefaultDocuments=a}return a},get_sharedFields:function(){ULSPjj:;var a=this.get_objectData().get_clientObjectProperties().SharedFields;if(SP.ScriptUtility.isUndefined(a)){a=new SP.DocumentSet.SharedFieldCollection(this.get_context(),new SP.ObjectPathProperty(this.get_context(),this.get_path(),"SharedFields"));this.get_objectData().get_clientObjectProperties().SharedFields=a}return a},get_welcomePageFields:function(){ULSPjj:;var a=this.get_objectData().get_clientObjectProperties().WelcomePageFields;if(SP.ScriptUtility.isUndefined(a)){a=new SP.DocumentSet.WelcomePageFieldCollection(this.get_context(),new SP.ObjectPathProperty(this.get_context(),this.get_path(),"WelcomePageFields"));this.get_objectData().get_clientObjectProperties().WelcomePageFields=a}return a},initPropertiesFromJson:function(b){ULSPjj:;SP.ClientObject.prototype.initPropertiesFromJson.call(this,b);var a;a=b.AllowedContentTypes;if(!SP.ScriptUtility.isUndefined(a)){this.updateClientObjectPropertyType("AllowedContentTypes",this.get_allowedContentTypes(),a);this.get_allowedContentTypes().fromJson(a);delete b.AllowedContentTypes}a=b.DefaultDocuments;if(!SP.ScriptUtility.isUndefined(a)){this.updateClientObjectPropertyType("DefaultDocuments",this.get_defaultDocuments(),a);this.get_defaultDocuments().fromJson(a);delete b.DefaultDocuments}a=b.SharedFields;if(!SP.ScriptUtility.isUndefined(a)){this.updateClientObjectPropertyType("SharedFields",this.get_sharedFields(),a);this.get_sharedFields().fromJson(a);delete b.SharedFields}a=b.WelcomePageFields;if(!SP.ScriptUtility.isUndefined(a)){this.updateClientObjectPropertyType("WelcomePageFields",this.get_welcomePageFields(),a);this.get_welcomePageFields().fromJson(a);delete b.WelcomePageFields}},update:function(a){ULSPjj:;var b=this.get_context(),c=new SP.ClientActionInvokeMethod(this,"Update",[a]);b.addQuery(c)}};SP.DocumentSet.DocumentSetTemplateObjectPropertyNames=function(){};SP.DocumentSet.SharedFieldCollection=function(b,a){ULSPjj:;SP.DocumentSet.SharedFieldCollection.initializeBase(this,[b,a])};SP.DocumentSet.SharedFieldCollection.prototype={itemAt:function(a){ULSPjj:;return this.getItemAtIndex(a)},get_item:function(a){ULSPjj:;return this.getItemAtIndex(a)},get_childItemType:function(){ULSPjj:;return SP.Field},add:function(c){ULSPjj:;var a=this.get_context(),b=new SP.ClientActionInvokeMethod(this,"Add",[c]);a.addQuery(b)},remove:function(c){ULSPjj:;var a=this.get_context(),b=new SP.ClientActionInvokeMethod(this,"Remove",[c]);a.addQuery(b)}};SP.DocumentSet.WelcomePageFieldCollection=function(b,a){ULSPjj:;SP.DocumentSet.WelcomePageFieldCollection.initializeBase(this,[b,a])};SP.DocumentSet.WelcomePageFieldCollection.prototype={itemAt:function(a){ULSPjj:;return this.getItemAtIndex(a)},get_item:function(a){ULSPjj:;return this.getItemAtIndex(a)},get_childItemType:function(){ULSPjj:;return SP.Field},add:function(c){ULSPjj:;var a=this.get_context(),b=new SP.ClientActionInvokeMethod(this,"Add",[c]);a.addQuery(b)},remove:function(a){ULSPjj:;var b=this.get_context(),c=new SP.ClientActionInvokeMethod(this,"Remove",[a]);b.addQuery(c)}};Type.registerNamespace("SP.Video");SP.Video.EmbedCodeConfiguration=function(){ULSPjj:;SP.Video.EmbedCodeConfiguration.initializeBase(this)};SP.Video.EmbedCodeConfiguration.prototype={$0_1:false,$1_1:false,$2_1:false,$3_1:false,$4_1:false,$5_1:0,$6_1:0,$7_1:null,$8_1:0,get_autoPlay:function(){ULSPjj:;return this.$0_1},set_autoPlay:function(a){ULSPjj:;this.$0_1=a;return a},get_displayTitle:function(){ULSPjj:;return this.$1_1},set_displayTitle:function(a){ULSPjj:;this.$1_1=a;return a},get_linkToOwnerProfilePage:function(){ULSPjj:;return this.$2_1},set_linkToOwnerProfilePage:function(a){ULSPjj:;this.$2_1=a;return a},get_linkToVideoHomePage:function(){ULSPjj:;return this.$3_1},set_linkToVideoHomePage:function(a){ULSPjj:;this.$3_1=a;return a},get_loop:function(){ULSPjj:;return this.$4_1},set_loop:function(a){ULSPjj:;this.$4_1=a;return a},get_pixelHeight:function(){ULSPjj:;return this.$5_1},set_pixelHeight:function(a){ULSPjj:;this.$5_1=a;return a},get_pixelWidth:function(){ULSPjj:;return this.$6_1},set_pixelWidth:function(a){ULSPjj:;this.$6_1=a;return a},get_previewImagePath:function(){ULSPjj:;return this.$7_1},set_previewImagePath:function(a){ULSPjj:;this.$7_1=a;return a},get_startTime:function(){ULSPjj:;return this.$8_1},set_startTime:function(a){ULSPjj:;this.$8_1=a;return a},get_typeId:function(){ULSPjj:;return"{294cf1eb-cef4-49e0-b114-648abb3916af}"},writeToXml:function(b,a){ULSPjj:;if(!b)throw Error.argumentNull("writer");if(!a)throw Error.argumentNull("serializationContext");var c=["AutoPlay","DisplayTitle","LinkToOwnerProfilePage","LinkToVideoHomePage","Loop","PixelHeight","PixelWidth","PreviewImagePath","StartTime"];SP.DataConvert.writePropertiesToXml(b,this,c,a);SP.ClientValueObject.prototype.writeToXml.call(this,b,a)},initPropertiesFromJson:function(b){ULSPjj:;SP.ClientValueObject.prototype.initPropertiesFromJson.call(this,b);var a;a=b.AutoPlay;if(!SP.ScriptUtility.isUndefined(a)){this.$0_1=a;delete b.AutoPlay}a=b.DisplayTitle;if(!SP.ScriptUtility.isUndefined(a)){this.$1_1=a;delete b.DisplayTitle}a=b.LinkToOwnerProfilePage;if(!SP.ScriptUtility.isUndefined(a)){this.$2_1=a;delete b.LinkToOwnerProfilePage}a=b.LinkToVideoHomePage;if(!SP.ScriptUtility.isUndefined(a)){this.$3_1=a;delete b.LinkToVideoHomePage}a=b.Loop;if(!SP.ScriptUtility.isUndefined(a)){this.$4_1=a;delete b.Loop}a=b.PixelHeight;if(!SP.ScriptUtility.isUndefined(a)){this.$5_1=a;delete b.PixelHeight}a=b.PixelWidth;if(!SP.ScriptUtility.isUndefined(a)){this.$6_1=a;delete b.PixelWidth}a=b.PreviewImagePath;if(!SP.ScriptUtility.isUndefined(a)){this.$7_1=a;delete b.PreviewImagePath}a=b.StartTime;if(!SP.ScriptUtility.isUndefined(a)){this.$8_1=a;delete b.StartTime}}};SP.Video.VideoSet=function(b,a){ULSPjj:;SP.Video.VideoSet.initializeBase(this,[b,a])};SP.Video.VideoSet.createVideo=function(a,d,f,e){ULSPjj:;if(!a)throw Error.argumentNull("context");var b,c=new SP.ClientActionInvokeStaticMethod(a,"{999f0b44-5022-4c04-a0c3-d0705e44395f}","CreateVideo",[d,f,e]);a.addQuery(c);b=new SP.StringResult;a.addQueryIdAndResultObject(c.get_id(),b);return b};SP.Video.VideoSet.uploadVideo=function(a,h,f,g,d,e){ULSPjj:;if(!a)throw Error.argumentNull("context");var b,c=new SP.ClientActionInvokeStaticMethod(a,"{999f0b44-5022-4c04-a0c3-d0705e44395f}","UploadVideo",[h,f,g,d,e]);a.addQuery(c);b=new SP.StringResult;a.addQueryIdAndResultObject(c.get_id(),b);return b};SP.Video.VideoSet.getEmbedCode=function(a,e,d){ULSPjj:;if(!a)throw Error.argumentNull("context");var b,c=new SP.ClientActionInvokeStaticMethod(a,"{999f0b44-5022-4c04-a0c3-d0705e44395f}","GetEmbedCode",[e,d]);a.addQuery(c);b=new SP.StringResult;a.addQueryIdAndResultObject(c.get_id(),b);return b};SP.Video.VideoSet.migrateVideo=function(a,c){ULSPjj:;if(!a)throw Error.argumentNull("context");var b;b=new SP.ListItem(a,new SP.ObjectPathStaticMethod(a,"{999f0b44-5022-4c04-a0c3-d0705e44395f}","MigrateVideo",[c]));return b};SP.DocumentManagement.MetadataDefaults.registerClass("SP.DocumentManagement.MetadataDefaults",SP.ClientObject);SP.DocumentSet.AllowedContentTypeCollection.registerClass("SP.DocumentSet.AllowedContentTypeCollection",SP.ClientObjectCollection);SP.DocumentSet.DefaultDocument.registerClass("SP.DocumentSet.DefaultDocument",SP.ClientObject);SP.DocumentSet.DefaultDocumentPropertyNames.registerClass("SP.DocumentSet.DefaultDocumentPropertyNames");SP.DocumentSet.DefaultDocumentCollection.registerClass("SP.DocumentSet.DefaultDocumentCollection",SP.ClientObjectCollection);SP.DocumentSet.DocumentSet.registerClass("SP.DocumentSet.DocumentSet",SP.ClientObject);SP.DocumentSet.DocumentSetTemplate.registerClass("SP.DocumentSet.DocumentSetTemplate",SP.ClientObject);SP.DocumentSet.DocumentSetTemplateObjectPropertyNames.registerClass("SP.DocumentSet.DocumentSetTemplateObjectPropertyNames");SP.DocumentSet.SharedFieldCollection.registerClass("SP.DocumentSet.SharedFieldCollection",SP.ClientObjectCollection);SP.DocumentSet.WelcomePageFieldCollection.registerClass("SP.DocumentSet.WelcomePageFieldCollection",SP.ClientObjectCollection);SP.Video.EmbedCodeConfiguration.registerClass("SP.Video.EmbedCodeConfiguration",SP.ClientValueObject);SP.Video.VideoSet.registerClass("SP.Video.VideoSet",SP.DocumentSet.DocumentSet);function sp_documentmanagement_initialize(){ULSPjj:;SP.DocumentSet.DefaultDocumentPropertyNames.contentTypeId="ContentTypeId";SP.DocumentSet.DefaultDocumentPropertyNames.documentPath="DocumentPath";SP.DocumentSet.DefaultDocumentPropertyNames.name="Name";SP.DocumentSet.DocumentSetTemplateObjectPropertyNames.allowedContentTypes="AllowedContentTypes";SP.DocumentSet.DocumentSetTemplateObjectPropertyNames.defaultDocuments="DefaultDocuments";SP.DocumentSet.DocumentSetTemplateObjectPropertyNames.sharedFields="SharedFields";SP.DocumentSet.DocumentSetTemplateObjectPropertyNames.welcomePageFields="WelcomePageFields"}sp_documentmanagement_initialize();RegisterModuleInit("sp.documentmanagement.js",sp_documentmanagement_initialize);typeof Sys!="undefined"&&Sys&&Sys.Application&&Sys.Application.notifyScriptLoaded();NotifyScriptLoadedAndExecuteWaitingJobs("SP.DocumentManagement.js");