<template>
  <div id="app">
    <el-container>
      <el-header class="header" v-show="showHeader">
        <el-menu class="el-menu-demo" mode="horizontal">
          <el-menu-item>
            <router-link to="/">主页</router-link>
          </el-menu-item>
          <el-menu-item>
            <router-link to="/about">我</router-link>
          </el-menu-item>
        </el-menu>
      </el-header>
      <el-main>
        <router-view></router-view>
      </el-main>
    </el-container>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import Cookies from "js-cookie"
export default Vue.extend({
  name: "App",
  data: function() {
    return {
      showHeader: true
    };
  },
  mounted: function() {
    this.$nextTick(function() {
      const url = window.location.href;
      if (url.indexOf("login") >= 0) {
        this.showHeader = false;
      }
      else {
        if(Cookies.get("userID")==undefined)
        {
          window.location.href="/login";
        }
      }
    });
  }
});
</script>
<style scoped>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}
header a {
  text-decoration: none;
}
</style>
