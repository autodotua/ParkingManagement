<template>
  <div id="app">
    <el-container>
      <!-- <el-header class="header" v-show="showHeader">
        <el-menu class="el-menu-demo" mode="horizontal">
          <el-menu-item>
            <router-link to="/">主页</router-link>
          </el-menu-item>
          <el-menu-item>
            <router-link to="/about">我</router-link>
          </el-menu-item>
        </el-menu>
      </el-header>-->

      <el-header class="header" v:show="showHeader">
        <el-button type="text" style="float:right" @click="clickUsername">{{username}}</el-button>
        <h3 style="float:left" @click="jump('')">停车场</h3>
        <slot name="header"></slot>
      </el-header>
      <el-main>
        <router-view></router-view>
      </el-main>
    </el-container>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import Cookies from "js-cookie";
import { jump } from "./common";
export default Vue.extend({
  name: "App",
  data: function() {
    return {
      showHeader: true
    };
  },
  computed: {
    username() {
      return Cookies.get("username");
    }
  },
  mounted: function() {
    this.$nextTick(function() {
      const url = window.location.href;
      if (url.indexOf("login") >= 0) {
        this.showHeader = false;
      } else {
        if (Cookies.get("userID") == undefined) {
         jump("login");
        }
      }
    });
  },
  methods: {
    jump:jump,
    clickUsername() {
      this.$confirm("是否退出账户？", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning",
      }).then(() => {
        Cookies.remove("username");
        Cookies.remove("userID");
        Cookies.remove("token");
        location.reload();
      });
    }
  }
});
</script>
<style >
.header-title {
  float: left;
  margin-top: 0px;
}.el-message-box{
  width:auto!important;
}
body{
  overflow-x: hidden;
}
</style>
<style scoped>
/* #app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}
header a {
  text-decoration: none;
} */

.header {
  margin-left: -12px;
  margin-right: -12px;
  margin-top: -12px;
  background: #ebeef5;
  color: #606266;
}

.header button {
  color: #606266;
  margin-top: 12px;
}

</style>
