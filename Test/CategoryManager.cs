﻿using System;
using System.Linq;
using NUnit.Framework;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using System.IO;
using System.Web;
using Sitecore.Modules.Blog.Items.Blog;
using System.Collections.Generic;
using Mod = Sitecore.Modules.Blog.Managers;
using Sitecore.Data;

namespace Sitecore.Modules.Blog.Test
{
    [TestFixture]
    [Category("CategoryManager")]
    public class CategoryManager
    {
        private Item m_testRoot = null;
        private Item m_blog1 = null;
        private Item m_category1 = null;
        private Item m_category2 = null;
        private Item m_category3 = null;
        private Item m_category4 = null;
        private Item m_entry1 = null;
        private Item m_entry2 = null;
        private Item m_comment1 = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // Create test content
            var home = Sitecore.Context.Database.GetItem("/sitecore/content/home");
            using (new SecurityDisabler())
            {
                // Don't change IDs as blog item references category by ID
                home.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\category manager content.xml")), false, PasteMode.Overwrite);
            }

            // Retrieve created content items
            m_testRoot = home.Axes.GetChild("blog test root");
            m_blog1 = m_testRoot.Axes.GetChild("blog1");

            var categories = m_blog1.Axes.GetChild("categories");
            m_category1 = categories.Axes.GetChild("category1");
            m_category2 = categories.Axes.GetChild("category2");
            m_category3 = categories.Axes.GetChild("category3");
            m_category4 = categories.Axes.GetChild("category4");

            m_entry1 = m_blog1.Axes.GetChild("entry1");
            m_entry2 = m_blog1.Axes.GetChild("entry2");

            m_comment1 = m_entry1.Axes.GetChild("comment1");
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            using (new SecurityDisabler())
            {
                if (m_testRoot != null)
                    m_testRoot.Delete();
            }
        }

        [Test]
        public void GetCategories_Null()
        {
            var categories = Mod.CategoryManager.GetCategories((Item)null);
            Assert.AreEqual(0, categories.Length);
        }

        [Test]
        public void GetCategories_OutsideBlogRoot()
        {
            var home = Sitecore.Context.Database.GetItem("/sitecore/content/home");
            var categories = Mod.CategoryManager.GetCategories(home);
            Assert.AreEqual(0, categories.Length);
        }

        [Test]
        public void GetCategories_OutsideBlogRoot_ById()
        {
            var home = Sitecore.Context.Database.GetItem("/sitecore/content/home");
            var categories = Mod.CategoryManager.GetCategories(home.ID.ToString());
            Assert.AreEqual(0, categories.Length);
        }

        [Test]
        public void GetCategories_OnBlogItem()
        {
            VerifyAllCategories(Mod.CategoryManager.GetCategories(m_blog1));
        }

        [Test]
        public void GetCategories_OnEntry1Item()
        {
            VerifyAllCategories(Mod.CategoryManager.GetCategories(m_entry1));
        }

        [Test]
        public void GetCategories_OnEntry1Item_ById()
        {
            VerifyAllCategories(Mod.CategoryManager.GetCategories(m_entry1.ID.ToString()));
        }

        [Test]
        public void GetCategories_OnEntry2Item()
        {
            VerifyAllCategories(Mod.CategoryManager.GetCategories(m_entry2));
        }

        [Test]
        public void GetCategories_OnCommentItem()
        {
            VerifyAllCategories(Mod.CategoryManager.GetCategories(m_comment1));
        }

        [Test]
        public void GetCategories_OnCommentItem_ById()
        {
            VerifyAllCategories(Mod.CategoryManager.GetCategories(m_comment1.ID.ToString()));
        }

        [Test]
        public void GetCategories_OnCategoryItem()
        {
            VerifyAllCategories(Mod.CategoryManager.GetCategories(m_category2));
        }

        private void VerifyAllCategories(CategoryItem[] categories)
        {
            var ids = (from category in categories
                       select category.ID).ToArray();

            Assert.AreEqual(4, ids.Length);
            Assert.Contains(m_category1.ID, ids);
            Assert.Contains(m_category2.ID, ids);
            Assert.Contains(m_category3.ID, ids);
            Assert.Contains(m_category4.ID, ids);
        }

        [Test]
        public void GetCategoriesByEntryID_NonEntryItem()
        {
            var categories = Mod.CategoryManager.GetCategoriesByEntryID(m_blog1.ID);
            Assert.AreEqual(0, categories.Length);
        }

        [Test]
        public void GetCategoriesByEntryID_Entry1()
        {
            var categories = Mod.CategoryManager.GetCategoriesByEntryID(m_entry1.ID);
            Assert.AreEqual(2, categories.Length);

            var ids = (from category in categories
                       select category.ID).ToArray();

            Assert.Contains(m_category1.ID, ids);
            Assert.Contains(m_category3.ID, ids);
        }

        [Test]
        public void GetCategoriesByEntryID_Entry2()
        {
            var categories = Mod.CategoryManager.GetCategoriesByEntryID(m_entry2.ID);
            Assert.AreEqual(2, categories.Length);

            var ids = (from category in categories
                       select category.ID).ToArray();

            Assert.Contains(m_category2.ID, ids);
            Assert.Contains(m_category3.ID, ids);
        }

        [Test]
        public void GetCategoriesByEntryID_InvalidID()
        {
            var categories = Mod.CategoryManager.GetCategoriesByEntryID(ID.NewID);
            Assert.AreEqual(0, categories.Length);
        }
    }
}