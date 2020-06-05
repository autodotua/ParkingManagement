<template>
  <div>
    <h2 class="header-title">修改密码</h2>
    <br />
    <br />

      <el-form ref="form" label-width="100px">
    <br />
        <el-form-item label="旧密码">
          <el-input v-model="oldPassword" type="password"></el-input>
        </el-form-item>
        <el-form-item label="新密码">
          <el-input v-model="newPassword1" type="password"></el-input>
        </el-form-item>
        <el-form-item label="重复新密码">
          <el-input v-model="newPassword2" type="password"></el-input>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="ok">设置</el-button>
        </el-form-item>
      </el-form>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { withToken, getUrl, showError,showSuccess, formatDateTime } from "../common";
import { Notification } from "element-ui";
export default Vue.extend({
  name: "Home",
  data() {
    return {
      oldPassword: "",
      newPassword1: "",
      newPassword2: ""
    };
  },
  methods: {
    ok() {
      if (this.newPassword1 != this.newPassword2) {
        showError("两次输入的密码不同！");
        return;
      }
      Vue.axios
        .post(
          getUrl("User", "Password"),
          withToken({
            oldPassword:this.oldPassword,
            newPassword:this.newPassword1
          })
        )
        .then(response => {
          if(response.data.succeed)
          {
            showSuccess("修改密码成功")
          }
          else{
            showError(response.data.message)
          }
        })
        .catch(showError);
    },
   
  },
  computed: {
   
  },
  components: {},
  // mounted: function() {
  //   this.$nextTick(function() {
     
  //   });
  // }
});
</script>

<style scoped>
.el-table .cell {
  white-space: pre-line;
  word-wrap: break-word;
}

.cell .el-button {
  margin-right: 6px;
}
</style>
