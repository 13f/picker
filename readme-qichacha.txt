【qichacha.com】

http://www.qichacha.com/material/theme/chacha/cms/v2/js/company.js?date=123
公司页面，切换Tab显示具体数据：
    function getDetail(tabid,unique,companyname){ 
    	unique      = encodeURIComponent(unique);	
        companyname = encodeURIComponent(companyname);		 	
		if(tabid=="base"||tabid==""||tabid==undefined||tabid=="comment"){
			path=INDEX_URL+"company_base?";
		}else if(tabid=="susong"){
			path=INDEX_URL+"company_susong?";
		}else if(tabid=="touzi"){
			path=INDEX_URL+"company_touzi?";
		}else if(tabid=="report"){ 
			path=INDEX_URL+"company_report?";
		}else if(tabid=="assets"){
			path=INDEX_URL+"company_assets?";
		}else if(tabid=="job"){
			path=INDEX_URL+"company_job?";
		}else if(tabid=="feeds"){
			path=INDEX_URL+"company_feeds?";
		}
		$("#load_data").show();		
		var url = path+"unique="+unique+'&companyname='+companyname;

1、根据id和法定名称获取公司基本信息：
http://qichacha.com/company_base?unique=185d28962dd320f863eff46fa4270dbe&companyname=北京格灵深瞳信息技术有限公司
unique是公司的id。

2、根据id和法定名称获取公司的对外投资信息：
	http://qichacha.com/company_touzi?unique=185d28962dd320f863eff46fa4270dbe&companyname=北京格灵深瞳信息技术有限公司
	INDEX_URL+"company_touzilist?"+"unique="+unique+"&companyname="+companyname+"&p="+page;

3、根据id和法定名称获取公司的企业年报：
http://qichacha.com/company_report?unique=185d28962dd320f863eff46fa4270dbe&companyname=北京格灵深瞳信息技术有限公司
获取年报（无效）：INDEX_URL+'/company_askReport?keyNo='+ unique

4、招聘：INDEX_URL+"company_joblist?"+"unique="+unique+"&companyname="+companyname+"&p="+page;
获取招聘信息：url:INDEX_URL+'/company_getByJob',
					data:'jobid='+JobId,

5、根据id和法定名称获取公司的无形资产：
http://qichacha.com/company_assets?unique=185d28962dd320f863eff46fa4270dbe&companyname=北京格灵深瞳信息技术有限公司
分页查询：商标：http://www.qichacha.com/company_shangbiao?unique=3f603703d59a04cbe427e5825099a565&companyname=百度在线网络技术（北京）有限公司&p=4
	INDEX_URL+"company_shangbiao?"+"unique="+unique+"&companyname="+companyname+"&p="+page;
	商标详情：url:INDEX_URL+'/company_shangbiaoView', 
				data:'id='+sbid,
				id放入URL请求（http://www.qichacha.com/company_shangbiaoView?id=TUQMTRULRNML）：Error：tmId不能为空
				id放入Cookie：
	商标html：shangbiaoHTML(data)
分页查询：专利：http://www.qichacha.com/company_zhuanli?unique=3f603703d59a04cbe427e5825099a565&companyname=百度在线网络技术（北京）有限公司&p=3
	INDEX_URL+"company_zhuanli?"+"unique="+unique+"&companyname="+companyname+"&p="+page;
	专利详情：url:INDEX_URL+'/company_zhuanliView', 
				data:'id='+zlid,
				id放入URL请求（http://www.qichacha.com/company_zhuanliView?id=P_SIPO-CN105427281A）：返回结果：{"data":"{","status":200}
				id放入Cookie：
	专利Html：zhuanliHTML(data)
分页查询：著作权：http://www.qichacha.com/company_zzq?unique=fa3d9de6fd3ccf354b5ae34c38eb7587&companyname=淘宝（中国）软件有限公司&p=2
	INDEX_URL+"company_zzq?"+"unique="+unique+"&companyname="+companyname+"&p="+page;
	著作权详情：url:INDEX_URL+'/company_zhuanliView', 
				data:'id='+zsid,
	著作权Html：zzqHTML(data)
分页查询：软件著作权：http://www.qichacha.com/company_rjzzq?unique=3f603703d59a04cbe427e5825099a565&companyname=百度在线网络技术（北京）有限公司&p=2
	INDEX_URL+"company_rjzzq?"+"unique="+unique+"&companyname="+companyname+"&p="+page;
分页查询：证书：http://www.qichacha.com/company_zhengshu?unique=9cce0780ab7644008b73bc2120479d31&companyname=小米科技有限责任公司&p=2
	INDEX_URL+"company_zhengshu?"+"unique="+unique+"&companyname="+companyname+"&p="+page;
	证书详情：url:INDEX_URL+'/company_zhuanliView', 
				data:'id='+zsid,
	证书Html：zhengshuHTML(data)
分页查询：网站信息：




6、根据id和法定名称获取公司的诉讼信息：http://qichacha.com/company_susong?unique=185d28962dd320f863eff46fa4270dbe&companyname=北京格灵深瞳信息技术有限公司
分页：执行人：INDEX_URL+"company_zhixing?"+"unique="+unique+"&companyname="+companyname+"&p="+page;
分页：失信人：INDEX_URL+"company_shixin?"+"unique="+unique+"&companyname="+companyname+"&p="+page;
分页：裁判文书：INDEX_URL+"company_wenshu?"+"unique="+unique+"&companyname="+companyname+"&p="+page;
	裁判文书详情：url:INDEX_URL+'/company_wenshuView', 
			data:'id='+wsid,
分页：公告：INDEX_URL+"company_gonggao?"+"unique="+unique+"&companyname="+companyname+"&p="+page;